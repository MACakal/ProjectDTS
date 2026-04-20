using StackExchange.Redis;

namespace ProjectDTS;

public class RatingService
{
    private readonly IDatabase _redisDb;

    public RatingService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public void AddOrUpdateRating(int productId, int userId, int ratingValue, string reviewText = null)
    {
        if (ratingValue < 1 || ratingValue > 5)
            throw new ArgumentException("Rating moet tussen 1 en 5 liggen.");


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
    public Rating GetUserRatingForProduct(int productId, int userId)
    {
        string reviewKey = $"review:{productId}:{userId}";
        var data = _redisDb.HashGetAll(reviewKey);

        if (data.Length == 0)
        {
            return null;
        }
        return MapFromHash(reviewKey, data, productId);
    }

    public int GetRatingCount(int productId)
    {
        string productIndexKey = $"product:{productId}:reviews";
        long count = _redisDb.SetLength(productIndexKey);
        return (int)count;
    }

    private Rating MapFromHash(string key, HashEntry[] entries, int productId)
    {
        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        int userId = int.Parse(key.Split(':')[2]);

        return new Rating {
            ProductId = productId,
            UserId = userId,
            RatingValue = int.Parse(dict["rating"]),
            ReviewText = dict["text"],
            CreatedAt = DateTime.Parse(dict["created_at"])
        };
    }
}