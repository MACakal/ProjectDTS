namespace ProjectDTS;

using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

using DotNetEnv;
using Neo4j.Driver;

public class Program
{
    public static async Task Main(string[] args)
    {

        UserSession.SessionId = Guid.NewGuid().ToString();

        Env.Load();
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var mongoContext = new MongoDbContext(configuration);
        var orderMongoService = new OrderMongoService(mongoContext);
        var userActionLogService = new UserActionLogService(mongoContext);
        var permissionChangeLogService = new PermissionChangeLogService(mongoContext);

        var databaseService = new DatabaseService();
        var redis = ConnectionMultiplexer.Connect(Env.GetString("REDIS_URL")); // or your connection string
        var ratingService = new RatingService(redis);


        //var ratingService = new RatingService(databaseService);
        var productAuditLogService = new ProductAuditLogService(mongoContext);
        var productService = new ProductService(databaseService, ratingService, productAuditLogService);

        var graphDb = new GraphDatabaseService();
        ///
        var graphProductService = new GraphProductService(graphDb.Driver, productService);



        var viewProduct = new ViewProductPres(productService, ratingService, userActionLogService);

        var filterMenu = new FilterMenu(productService, viewProduct, userActionLogService);

        var userService = new UserService(databaseService);
        var roleService = new RoleService(databaseService, permissionChangeLogService);        
        var basketService = new BasketService(databaseService, orderMongoService, userActionLogService);

        var accountPresentation = new AccountPre(userService);

        var customerMenu = new CustomerMenuPre(viewProduct, filterMenu, accountPresentation);
        var graphservice = new Graphservice("bolt://localhost:7687", "neo4j", Env.GetString("NEO4J_PASSWORD"));
        var adminMenuPres = new AdminMenuPres(productService, viewProduct, userService, ratingService, graphservice, roleService, userActionLogService);
        var mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres, userService, viewProduct, roleService);
        var sortingMenu = new SortingMenu(productService, viewProduct);
        var basketMenu = new BasketMenu(
            productService,
            filterMenu,
            basketService,
            sortingMenu,
            ratingService,
            userActionLogService,
            userService,
            graphProductService
        );
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
