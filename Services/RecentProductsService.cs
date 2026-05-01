using StackExchange.Redis;
public class RecentProductsService
{
    private readonly IDatabase _db;

    public RecentProductsService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private string GetKey(int userId)
    {
        return $"recent:user:{userId}";
    }

    public async Task AddViewedProduct(int userId, int productId)
    {
        var key = GetKey(userId);
        var value = productId.ToString();

        await _db.ListRemoveAsync(key, value);

        await _db.ListLeftPushAsync(key, value);

        await _db.ListTrimAsync(key, 0, 9);
    }

    public async Task<List<int>> GetRecentProductIds(int userId)
    {
        var key = GetKey(userId);

        var values = await _db.ListRangeAsync(key, 0, -1);

        return values.Select(v => (int)v).ToList();
    }
}
