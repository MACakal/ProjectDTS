using Npgsql;

namespace ProjectDTS;

public class RatingService
{
    private readonly DatabaseService _db;

    public RatingService(DatabaseService db)
    {
        _db = db;
    }

    public void AddOrUpdateRating(int productId, int userId, int ratingValue, string reviewText = null)
    {
        if (ratingValue < 1 || ratingValue > 5)
            throw new ArgumentException("Rating value must be between 1 and 5.");

        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            INSERT INTO ratings (product_id, user_id, rating_value, review_text)
            VALUES (@product_id, @user_id, @rating_value, @review_text)
            ON CONFLICT (product_id, user_id)
            DO UPDATE SET rating_value = @rating_value, review_text = @review_text;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("product_id", productId);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("rating_value", ratingValue);
        cmd.Parameters.AddWithValue("review_text", (object?)reviewText ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }

    public List<Rating> GetProductRatings(int productId)
    {
        var ratings = new List<Rating>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT id, product_id, user_id, rating_value, review_text, created_at
            FROM ratings
            WHERE product_id = @product_id
            ORDER BY created_at DESC;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("product_id", productId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            ratings.Add(MapRating(reader));
        }
        return ratings;
    }

    public double GetAverageRating(int productId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT COALESCE(AVG(rating_value), 0) as avg_rating
            FROM ratings
            WHERE product_id = @product_id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("product_id", productId);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToDouble(result) : 0.0;
    }

    public int GetRatingCount(int productId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT COUNT(*) as rating_count
            FROM ratings
            WHERE product_id = @product_id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("product_id", productId);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public Rating GetUserRatingForProduct(int productId, int userId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT id, product_id, user_id, rating_value, review_text, created_at
            FROM ratings
            WHERE product_id = @product_id AND user_id = @user_id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("product_id", productId);
        cmd.Parameters.AddWithValue("user_id", userId);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return MapRating(reader);
        }
        return null;
    }

    public void DeleteRating(int ratingId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"DELETE FROM ratings WHERE id = @id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", ratingId);

        cmd.ExecuteNonQuery();
    }

    public List<ReviewAdminView> GetAllRatingsWithDetails()
    {
        var reviews = new List<ReviewAdminView>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
            SELECT r.id, p.name AS product_name, u.name AS user_name,
                   r.rating_value, r.review_text, r.created_at
            FROM ratings r
            JOIN products p ON p.id = r.product_id
            JOIN users u ON u.id = r.user_id
            ORDER BY r.created_at DESC;";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            reviews.Add(new ReviewAdminView
            {
                RatingId = (int)reader["id"],
                ProductName = (string)reader["product_name"],
                UserName = (string)reader["user_name"],
                RatingValue = (int)reader["rating_value"],
                ReviewText = reader["review_text"] != DBNull.Value ? (string)reader["review_text"] : null,
                CreatedAt = (DateTime)reader["created_at"]
            });
        }
        return reviews;
    }

    private Rating MapRating(NpgsqlDataReader reader)
    {
        return new Rating
        {
            Id = (int)reader["id"],
            ProductId = (int)reader["product_id"],
            UserId = (int)reader["user_id"],
            RatingValue = (int)reader["rating_value"],
            ReviewText = reader["review_text"] != DBNull.Value ? (string)reader["review_text"] : null,
            CreatedAt = (DateTime)reader["created_at"]
        };
    }
}
