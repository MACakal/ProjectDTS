using Npgsql;
namespace ProjectDTS;

public class ProductService
{
    private readonly DatabaseService _db;

    public ProductService(DatabaseService db)
    {
        _db = db;
    }

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection(); // make a connection with database (doesn't open it yet)
        conn.Open();// open connection

        string sql = @"SELECT id, name,  
             description, category, price, rarity
            FROM products";

        using var cmd = new NpgsqlCommand(sql, conn); // create SQL command linked to connection
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var product = new Product
            {

                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Category = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Price = reader.GetDecimal(4),
                Rarity = reader.IsDBNull(5) ? "" : reader.GetString(5)
            };


            products.Add(product);
        }
        return products;
    }

    public void AddProduct(Product product)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"INSERT INTO products 
                   (name, description, category, price, rarity)
                   VALUES (@name, @description, @category, @price, @rarity)";

        using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("name", product.Name);
        cmd.Parameters.AddWithValue("description", (object?)product.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("category", (object?)product.Category ?? DBNull.Value);
        cmd.Parameters.AddWithValue("price", product.Price);
        cmd.Parameters.AddWithValue("rarity", (object?)product.Rarity ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }
}
