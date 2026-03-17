using Npgsql;

namespace ProjectDTS;

public class BasketService
{
    private readonly DatabaseService _db;

    public BasketService(DatabaseService db)
    {
        _db = db;
    }

    public bool ModifyQuantityBasket(int userId, int productId, int newQuantity)
    {
        if (newQuantity <= 0)
            return false;

        using var conn = _db.GetConnection();
        conn.Open();

        string getOrderSql = @"
            SELECT id 
            FROM orders 
            WHERE user_id = @userId AND purchased = false
            LIMIT 1;";

        int? orderId;
        using (var cmdOrder = new NpgsqlCommand(getOrderSql, conn))
        {
            cmdOrder.Parameters.AddWithValue("userId", userId);
            var result = cmdOrder.ExecuteScalar();
            orderId = result == null ? null : (int?)result;
        }

        if (orderId == null)
            return false;

        string updateSql = @"
            UPDATE order_items
            SET quantity = @quantity
            WHERE order_id = @orderId AND product_id = @productId;";

        using var cmdUpdate2 = new NpgsqlCommand(updateSql, conn);
        cmdUpdate2.Parameters.AddWithValue("orderId", orderId.Value);
        cmdUpdate2.Parameters.AddWithValue("productId", productId);
        cmdUpdate2.Parameters.AddWithValue("quantity", newQuantity);

        int rowsAffected = cmdUpdate2.ExecuteNonQuery();
        if (rowsAffected == 0)
            return false;

        string updateTotalSql = @"
            UPDATE orders
            SET total_price = COALESCE((
                SELECT SUM(oi.quantity * p.price)
                FROM order_items oi
                JOIN products p ON oi.product_id = p.id
                WHERE oi.order_id = @orderId
            ), 0)
            WHERE id = @orderId;";

        using var cmdTotal = new NpgsqlCommand(updateTotalSql, conn);
        cmdTotal.Parameters.AddWithValue("orderId", orderId.Value);
        cmdTotal.ExecuteNonQuery();

        return true;
    }
    public bool RemoveFromBasket(int userId, int productId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string getOrderSql = @"
            SELECT id 
            FROM orders 
            WHERE user_id = @userId AND purchased = false
            LIMIT 1;";

        int? orderId;
        using (var cmdOrder = new NpgsqlCommand(getOrderSql, conn))
        {
            cmdOrder.Parameters.AddWithValue("userId", userId);
            var result = cmdOrder.ExecuteScalar();
            orderId = result == null ? null : (int?)result;
        }

        if (orderId == null)
        {
            return false;
        }

        string deleteSql = @"
            DELETE FROM order_items
            WHERE order_id = @orderId AND product_id = @productId;";
        using var cmdDelete = new NpgsqlCommand(deleteSql, conn);
        cmdDelete.Parameters.AddWithValue("orderId", orderId.Value);
        cmdDelete.Parameters.AddWithValue("productId", productId);

        // bereken nieuwe totaal, 0 als het de laatste item was
        string updateTotalSql = @" 
            UPDATE orders
            SET total_price = COALESCE((
                SELECT SUM(oi.quantity * p.price)
                FROM order_items oi
                JOIN products p ON oi.product_id = p.id
                WHERE oi.order_id = @orderId
            ), 0)
            WHERE id = @orderId;";
        using var cmdTotal = new NpgsqlCommand(updateTotalSql, conn);
        cmdTotal.Parameters.AddWithValue("orderId", orderId.Value);
        int rowsAffected = cmdDelete.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            return false;
        }
        return true;
    }

    public void AddToBasket(int userId, int productId, int quantity)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        // 1. Zorg dat (user_id) expliciet tussen haakjes staat voor de match met de index
        string getOrderSql = @"
            INSERT INTO orders (user_id, purchased, total_price)
            VALUES (@userId, false, 0)
            ON CONFLICT (user_id) WHERE (purchased = false) 
            DO UPDATE SET user_id = EXCLUDED.user_id
            RETURNING id;";

        int orderId;
        using (var cmdOrder = new NpgsqlCommand(getOrderSql, conn))
        {
            cmdOrder.Parameters.AddWithValue("userId", userId);
            // ExecuteScalar haalt de 'RETURNING id' op
            orderId = (int)cmdOrder.ExecuteScalar();
        }

        // 2. Deze query zou nu moeten werken omdat de constraint 'unique_order_product' bestaat
        string sqlItem = @"
            INSERT INTO order_items (order_id, product_id, quantity) 
            VALUES (@orderId, @productId, @quantity)
            ON CONFLICT (order_id, product_id) 
            DO UPDATE SET quantity = order_items.quantity + EXCLUDED.quantity;";

        using var cmdItem = new NpgsqlCommand(sqlItem, conn);
        cmdItem.Parameters.AddWithValue("orderId", orderId);
        cmdItem.Parameters.AddWithValue("productId", productId);
        cmdItem.Parameters.AddWithValue("quantity", quantity);
        cmdItem.ExecuteNonQuery();

        string updateTotalSql = @"
        UPDATE orders 
        SET total_price = (
            SELECT SUM(oi.quantity * p.price) 
            FROM order_items oi 
            JOIN products p ON oi.product_id = p.id 
            WHERE oi.order_id = @orderId
        )
        WHERE id = @orderId;";

        using var cmdUpdate = new NpgsqlCommand(updateTotalSql, conn);
        cmdUpdate.Parameters.AddWithValue("orderId", orderId);
        cmdUpdate.ExecuteNonQuery();
    }
    public List<BasketItem> GetBasketLines(int userId, out decimal total)
    {
        var items = new List<BasketItem>();
        total = 0;

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.id, p.name, oi.quantity, p.price
            FROM orders o
            JOIN order_items oi ON o.id = oi.order_id
            JOIN products p ON oi.product_id = p.id
            WHERE o.user_id = @userId AND o.purchased = false";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var item = new BasketItem
            {
                ProductId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Quantity = reader.GetInt32(2),
                Price = reader.GetDecimal(3)
            };

            total += item.Subtotal;
            items.Add(item);
        }

        return items;
    }
    public bool CheckoutWithTransaction(int userId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {
            string updateOrderSql = @"
                UPDATE orders 
                SET purchased = true 
                WHERE user_id = @userId AND purchased = false
                RETURNING id;";

            using var cmd = new NpgsqlCommand(updateOrderSql, conn, transaction);
            cmd.Parameters.AddWithValue("userId", userId);
            
            var orderId = cmd.ExecuteScalar();

            if (orderId == null)
            {
                transaction.Rollback();
                return false;
            }

            // 2. Optioneel: Hier zou je voorraad-updates kunnen doen voor elk product in order_items
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            // Bij een fout draaien we alles terug naar de oude staat
            transaction.Rollback();
            Console.WriteLine($"Error during checkout: {ex.Message}");
            return false;
        }
    }

    public bool Checkout(int userId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            UPDATE orders 
            SET purchased = true 
            WHERE user_id = @userId AND purchased = false
            RETURNING id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);
    
        var result = cmd.ExecuteScalar();
        return result != null;
    }

    public List<string> GetPastOrderLinesLastMonth(int userId, out decimal total)
    {
        var lines = new List<string>();
        total = 0;

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.name, oi.quantity, p.price
            FROM orders o
            JOIN order_items oi ON o.id = oi.order_id
            JOIN products p ON oi.product_id = p.id
            WHERE o.user_id = @userId
            AND o.purchased = true
            AND o.created_at >= CURRENT_TIMESTAMP - INTERVAL '1 month';";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("userId", userId);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string name = reader.GetString(0);
            int qty = reader.GetInt32(1);
            decimal price = reader.GetDecimal(2);
            decimal subtotal = qty * price;

            total += subtotal;

            lines.Add($"- {name,-20} | {qty}x | €{subtotal:N2}");
        }

        return lines;
    }
}