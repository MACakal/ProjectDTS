using Npgsql;

namespace ProjectDTS;

public class ProductService
{
    private readonly DatabaseService _db;
    private readonly RatingService _ratingService;

    public ProductService(DatabaseService db, RatingService ratingService)
    {
        _db = db;
        _ratingService = ratingService;
    }

    public List<Product> GetAllProducts()
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * FROM products";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
        return products;
    }



    public Product[] GetProductsByRange(decimal min, decimal max)
    {
        if (min > max)
            throw new ArgumentException("Minimum price cannot be greater than maximum price.");

        var products = new List<Product>();

        using var connection = _db.GetConnection();
        connection.Open();

        const string sql = @"
            SELECT *
            FROM products
            WHERE price BETWEEN @min AND @max
            ORDER BY price ASC;";

        using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("min", NpgsqlTypes.NpgsqlDbType.Numeric, min);
        command.Parameters.AddWithValue("max", NpgsqlTypes.NpgsqlDbType.Numeric, max);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
        return products.ToArray();
    }

    public List<Product> GetProductsByCategory(string categoryName)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * FROM products WHERE category = @cat";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("cat", categoryName);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
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
        string sql = $@"SELECT * FROM products ORDER BY price {direction}";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
        return products;
    }

    public List<Product> GetProductsSortedByPopularity(bool ascending)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();


        string direction = ascending ? "ASC" : "DESC";
        string sql = $@"SELECT * FROM products ORDER BY purchase_count {direction}";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
        return products;
    }

    public List<Product> SearchProductsByName(string searchTerm)
    {
        var products = new List<Product>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * 
                    FROM products 
                    WHERE name ILIKE @term"; // i like zorgt voor case-insensitive zoeken

        using var cmd = new NpgsqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("term", $"%{searchTerm}%"); // de % -woord- % betekend alles wat searchterm bevat geef dat terug

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        PopulateRatings(products);
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
            Rarity = reader.IsDBNull(5) ? "" : reader.GetString(5),
            View_count = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
            Purchase_count = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
            Stock = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
            AverageRating = 0.0,
            RatingCount = 0
        };
    }

    private void PopulateRatings(List<Product> products)
    {
        foreach (var product in products)
        {
            product.AverageRating = _ratingService.GetAverageRating(product.Id);
            product.RatingCount = _ratingService.GetRatingCount(product.Id);
        }
    }

    public void UpdateProduct(Product product)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
        UPDATE products
        SET name = @name, 
            description = @description,
            category = @category,
            price = @price,
            rarity = @rarity
        WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", product.Id);
        cmd.Parameters.AddWithValue("name", product.Name);
        cmd.Parameters.AddWithValue("description", product.Description);
        cmd.Parameters.AddWithValue("category", product.Category);
        cmd.Parameters.AddWithValue("price", product.Price);
        cmd.Parameters.AddWithValue("rarity", product.Rarity);

        cmd.ExecuteNonQuery();
    }
    public Product? GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = "SELECT * FROM products WHERE id=@id";

        var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);

        var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var product = MapProduct(reader);
            PopulateRatings(new List<Product> { product });
            return product;
        }

        return null;
    }

    public List<Product> GetTop3ChetProducts()
    {
        var products = new List<Product>();

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * FROM top3_cheapest";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }

        PopulateRatings(products);
        return products;
    }

    public List<Product> GetTop3ExpProducts()
    {
        var products = new List<Product>();

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * FROM top3_expensive";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }

        PopulateRatings(products);
        return products;
    }

    public List<(string Category, int TotalPurchases)> GetPopularCategories(DateTime start, DateTime end)
    {
        var categories = new List<(string, int)>();

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT p.category, SUM(oi.quantity) AS total_purchases
            FROM orders o
            JOIN order_items oi ON o.id = oi.order_id
            JOIN products p ON oi.product_id = p.id
            WHERE o.purchased = true
            AND o.created_at BETWEEN @start AND @end
            GROUP BY p.category
            ORDER BY total_purchases DESC;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("start", start);
        cmd.Parameters.AddWithValue("end", end);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            categories.Add((
                reader.IsDBNull(0) ? "" : reader.GetString(0),
                reader.GetInt32(1)
            ));
        }

        return categories;
    }


    public bool DeleteProduct(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = @"DELETE FROM products WHERE id=@id;";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);

        try
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {

            throw new Exception("Cannot delete product because it is used in orders.");
        }

    }

    public List<Product> GetProductsPerCategory()
    {
        var products = new List<Product>();

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
        WITH RankedProducts AS(
        SELECT
         name,
         category,
         price, 
         ROW_NUMBER() OVER (
         PARTITION BY category
         ORDER BY price DESC
         ) AS rn
         FROM products
        )
        SELECT name, category, price
        FROM RankedProducts WHERE rn <= 3;
         ";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            var product = new Product
            {
                Name = reader.GetString(0),
                Category = reader.GetString(1),
                Price = reader.GetDecimal(2)
            };
            products.Add(product);
        }
        return products;

    }

}