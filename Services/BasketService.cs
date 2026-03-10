using Npgsql;
using NpgsqlTypes;
namespace ProjectDTS;
public class BasketService
{
    private readonly DatabaseService _db;

    public BasketService(DatabaseService db)
    {
        _db = db;
    }

    public void AddToBasket(int userId, int productId, int quantity = 1)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string getOrderSql = @"
            INSERT INTO orders (user_id, purchased, total_price, created_at)
            VALUES (@userId, false, 0, NOW())
            ON CONFLICT (user_id) WHERE purchased = false DO UPDATE SET user_id = EXCLUDED.user_id
            RETURNING id;";
    

        int orderId = GetOrCreateOpenOrderId(conn, userId);

        string sql = @"
            INSERT INTO order_items (order_id, product_id, quantity) 
            VALUES (@orderId, @productId, @quantity)
            ON CONFLICT (order_id, product_id) 
            DO UPDATE SET quantity = order_items.quantity + EXCLUDED.quantity;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("orderId", orderId);
        cmd.Parameters.AddWithValue("productId", productId);
        cmd.Parameters.AddWithValue("quantity", quantity);
        cmd.ExecuteNonQuery();
    }

    private int GetOrCreateOpenOrderId(NpgsqlConnection conn, int userId)
    {
        // 1. Probeer eerst een bestaande 'basket' (order) te vinden
        string findSql = "SELECT id FROM orders WHERE user_id = @userId AND purchased = false LIMIT 1";
        using (var findCmd = new NpgsqlCommand(findSql, conn))
        {
            findCmd.Parameters.AddWithValue("userId", userId);
            var result = findCmd.ExecuteScalar();

            if (result != null)
            {
                return (int)result; // Gevonden! Geef het ID terug.
            }
        }

        // 2. Als er geen open order is, maak er eentje aan
        string insertSql = @"
            INSERT INTO orders (user_id, purchased, total_price, created_at) 
            VALUES (@userId, false, 0, NOW()) 
            RETURNING id";
        
        using (var insertCmd = new NpgsqlCommand(insertSql, conn))
        {
            insertCmd.Parameters.AddWithValue("userId", userId);
            return (int)insertCmd.ExecuteScalar(); // Geeft het nieuwe ID van de database terug
        }
    }
}