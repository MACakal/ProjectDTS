namespace ProjectDTS;
using StackExchange.Redis;

using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
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
        var basketService = new BasketService(databaseService);
        
        var accountPresentation = new AccountPre(userService);

        var customerMenu = new CustomerMenuPre(viewProduct, filterMenu, accountPresentation);
        var adminMenuPres = new AdminMenuPres(productService, viewProduct, userService, ratingService);

        var mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres, userService, viewProduct);
        var sortingMenu = new SortingMenu(productService, viewProduct);
        var basketMenu = new BasketMenu(productService, filterMenu, basketService, sortingMenu, ratingService);
        mainMenuPre.Show();
    }
}