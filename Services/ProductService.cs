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
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT id, name, description, category, price, rarity FROM products";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        return products;
    }

    public List<Product> GetProductsByCategory(string categoryName)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT id, name, description, category, price, rarity 
                       FROM products 
                       WHERE category = @cat";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("cat", categoryName);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
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
    public List<Product> GetProductsSortedByPrice(bool ascending)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();


        string direction = ascending ? "ASC" : "DESC";
        string sql = $@"SELECT id, name, description, category, price, rarity 
                        FROM products 
                        ORDER BY price {direction}";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {

            products.Add(MapProduct(reader));
        }
        return products;
    }
    public List<Product> SearchProductsByName(string searchTerm)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT id, name, description, category, price, rarity 
                    FROM products 
                    WHERE name ILIKE @term"; // i like zorgt voor case-insensitive zoeken

        using var cmd = new NpgsqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("term", $"%{searchTerm}%"); // de % -woord- % betekend alles wat searchterm bevat geef dat terug

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        return products;
    }

    // Kleine hulp-methode om dubbele code te voorkomen
    private Product MapProduct(NpgsqlDataReader reader)
    {
        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
            Category = reader.IsDBNull(3) ? "" : reader.GetString(3),
            Price = reader.GetDecimal(4),
            Rarity = reader.IsDBNull(5) ? "" : reader.GetString(5)
        };
    }

}