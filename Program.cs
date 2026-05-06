namespace ProjectDTS;

using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();

        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var mongoContext = new MongoDbContext(configuration);
        var orderMongoService = new OrderMongoService(mongoContext);

        var databaseService = new DatabaseService();
        var redis = ConnectionMultiplexer.Connect(Env.GetString("REDIS_URL")); // or your connection string
        var ratingService = new RatingService(redis);

        // Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("1234"));
        // using (var conn = databaseService.GetConnection())
        // {
        //     conn.Open();
        //     Console.WriteLine("CONNECTED");
        // }

        //var ratingService = new RatingService(databaseService);
        var productService = new ProductService(databaseService, ratingService);

        var viewProduct = new ViewProductPres(productService, ratingService);

        var filterMenu = new FilterMenu(productService, viewProduct);

        var userService = new UserService(databaseService);
        var basketService = new BasketService(databaseService, orderMongoService);

        var accountPresentation = new AccountPre(userService);

        var customerMenu = new CustomerMenuPre(viewProduct, filterMenu, accountPresentation);
        var adminMenuPres = new AdminMenuPres(productService, viewProduct, userService, ratingService);

        var mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres, userService, viewProduct);
        var sortingMenu = new SortingMenu(productService, viewProduct);
        var basketMenu = new BasketMenu(productService, filterMenu, basketService, sortingMenu, ratingService);
        mainMenuPre.Show();
    }
}

// using MongoDB.Driver;

// public class Program
// {
//     public static async Task Main(string[] args)
//     {
//         var connectionString = "mongodb+srv://baselkhrbeet_db_user:t01SvMVVup3mBzdy@cluster0.xn2hzkq.mongodb.net/?appName=Cluster0";
//         var databaseName = "webshop";

//         var client = new MongoClient(connectionString);
//         var db = client.GetDatabase(databaseName);

//         var collections = await db.ListCollectionNames().ToListAsync();

//         Console.WriteLine("Connected to MongoDB ✅");
//         Console.WriteLine($"Collections count: {collections.Count}");
//     }
// }