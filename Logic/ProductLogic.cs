using ProjectDTS;
using StackExchange.Redis;
using DotNetEnv;

public class ProductLogic
{
    // private static readonly RatingService _ratingService = new RatingService(ConnectionMultiplexer.Connect(Env.GetString("REDIS_URL")));
    private static readonly ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect(Env.GetString("REDIS_URL"));

    private static readonly RecentProductsService _recentService = new RecentProductsService(_redis);
    private static readonly RatingService _ratingService = new RatingService(_redis);
    private static readonly ProductService _productService = new ProductService(new DatabaseService(), _ratingService);

    public List<Product> GetTop3ChetProducts()
    {
        return _productService.GetTop3ChetProducts();
    }

    public List<Product> GetTop3ExpProducts()
    {
        return _productService.GetTop3ExpProducts();
    }

    public async Task<Product?> GetProductAndTrack(int userId, int productId)
    {
        var product = _productService.GetById(productId);

        if (product != null)
        {
            await _recentService.AddViewedProduct(userId, productId);
        }

        return product;
    }

    public async Task<List<Product>> GetRecentProducts(int userId)
    {
        var ids = await _recentService.GetRecentProductIds(userId);

        var result = new List<Product>();

        foreach (var id in ids)
        {
            var product = _productService.GetById(id);

            if (product != null)
                result.Add(product);
        }
        return result;
    }
}