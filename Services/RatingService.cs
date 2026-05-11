using StackExchange.Redis;

namespace ProjectDTS;

public class RatingService
{
    private readonly IDatabase _redisDb;
    private readonly UserService _userService = new UserService(new DatabaseService());

    public RatingService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public void AddOrUpdateRating(int productId, int userId, int ratingValue, string reviewText = null)
    {
        if (ratingValue < 1 || ratingValue > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (!_userService.HasPurchasedProduct(userId, productId))
            throw new Exception("You can only review products you have purchased.");

        string reviewKey = $"review:{productId}:{userId}";
        string productIndexKey = $"product:{productId}:reviews";

        HashEntry[] reviewData = {
            new HashEntry("rating", ratingValue),
            new HashEntry("text", reviewText ?? ""),
            new HashEntry("created_at", DateTime.UtcNow.ToString())
        };

        _redisDb.HashSet(reviewKey, reviewData);
        _redisDb.SetAdd(productIndexKey, reviewKey);
    }

    public double GetAverageRating(int productId)
    {
        var ratings = GetProductRatings(productId);
        if (ratings.Count == 0) return 0.0;
        return ratings.Average(r => r.RatingValue);
    }

    public int GetRatingCount(int productId)
    {
        string productIndexKey = $"product:{productId}:reviews";
        return (int)_redisDb.SetLength(productIndexKey);
    }

    public List<Rating> GetProductRatings(int productId)
    {
        var ratings = new List<Rating>();
        string productIndexKey = $"product:{productId}:reviews";

        var reviewKeys = _redisDb.SetMembers(productIndexKey);

        foreach (var key in reviewKeys)
        {
            var data = _redisDb.HashGetAll(key.ToString());
            if (data.Length > 0)
            {
                ratings.Add(MapFromHash(key.ToString(), data, productId));
            }
        }
        return ratings;
    }

    public Rating? GetUserRatingForProduct(int productId, int userId)
    {
        string reviewKey = $"review:{productId}:{userId}";
        var data = _redisDb.HashGetAll(reviewKey);

        if (data.Length == 0) return null;

        return MapFromHash(reviewKey, data, productId);
    }

    private Rating MapFromHash(string key, HashEntry[] entries, int productId)
    {
        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        int userId = int.Parse(key.Split(':')[2]);

        return new Rating
        {
            ProductId = productId,
            UserId = userId,
            RatingValue = int.Parse(dict["rating"]),
            ReviewText = dict["text"],
            CreatedAt = DateTime.Parse(dict["created_at"])
        };
    }

    public List<RatingDetail> GetAllRatingsWithDetails()
    {
        var result = new List<RatingDetail>();

        var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints()[0]);
        var keys = server.Keys(pattern: "review:*:*");

        foreach (var key in keys)
        {
            var data = _redisDb.HashGetAll(key);
            if (data.Length == 0) continue;

            var parts = key.ToString().Split(':');
            int productId = int.Parse(parts[1]);
            int userId = int.Parse(parts[2]);

            var dict = data.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

            result.Add(new RatingDetail
            {
                RatingKey = key.ToString(),
                ProductId = productId,
                UserId = userId,
                UserName = _userService.GetUserById(userId)?.Name ?? "Unknown",
                ProductName = GetProductName(productId),
                RatingValue = int.Parse(dict["rating"]),
                ReviewText = dict["text"],
                CreatedAt = DateTime.Parse(dict["created_at"])
            });
        }

        return result;
    }

    public void DeleteRating(string ratingKey)
    {
        var parts = ratingKey.Split(':');
        int productId = int.Parse(parts[1]);

        string productIndexKey = $"product:{productId}:reviews";
        _redisDb.SetRemove(productIndexKey, ratingKey);
        _redisDb.KeyDelete(ratingKey);
    }

    private string GetProductName(int productId)
    {
        string key = $"product:{productId}:name";
        var name = _redisDb.StringGet(key);
        return name.HasValue ? name.ToString() : $"Product {productId}";
    }
}