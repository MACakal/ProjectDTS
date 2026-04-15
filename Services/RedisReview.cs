public interface IReviewRepository
{
    void Save(Review review);
    List<Review> GetByProductId(string productId);
    void Delete(string reviewId);
}

public class RedisReview : IReviewRepository
{
    private readonly IDatabase _db;
    private const string KeyPrefix = "review:";

    public RedisReview(string connectionString = "127.0.0.1:6379")
    {
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _db = redis.GetDatabase();
    }

    public void Save(Review review)
    {
        string key = KeyPrefix + review.ID;
        string json = JsonConvert.SerializeObject(review);
        _db.StringSet(key, json);

        // index by productId so we can fetch all reviews per product
        _db.SetAdd($"product:{review.ProductID}:reviews", review.ID);
    }

    public List<Review> GetByProductId(string productId)
    {
        var ids = _db.SetMembers($"product:{productId}:reviews");
        var reviews = new List<Review>();

        foreach (var id in ids)
        {
            var json = _db.StringGet(KeyPrefix + id);
            if (json.HasValue)
                reviews.Add(JsonConvert.DeserializeObject<Review>(json));
        }

        return reviews;
    }

    public void Delete(string reviewId)
    {
        var json = _db.StringGet(KeyPrefix + reviewId);
        if (json.HasValue)
        {
            var review = JsonConvert.DeserializeObject<Review>(json);
            _db.SetRemove($"product:{review.ProductID}:reviews", reviewId);
        }
        _db.KeyDelete(KeyPrefix + reviewId);
    }
}