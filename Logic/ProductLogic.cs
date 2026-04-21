using ProjectDTS;
using StackExchange.Redis;
using DotNetEnv;

public class ProductLogic
{
    private static readonly RatingService _ratingService = new RatingService(ConnectionMultiplexer.Connect(Env.GetString("REDIS_URL")));
    private static readonly ProductService _productService = new ProductService(new DatabaseService(), _ratingService);

    public List<Product> GetTop3ChetProducts()
    {
        return _productService.GetTop3ChetProducts();
    }

    public List<Product> GetTop3ExpProducts()
    {
        return _productService.GetTop3ExpProducts();
    }
}