namespace ProjectDTS;

using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var databaseService = new DatabaseService();


        // using (var conn = databaseService.GetConnection())
        // {
        //     conn.Open();
        //     Console.WriteLine("CONNECTED");
        // }
        var productService = new ProductService(databaseService);

        var viewProduct = new ViewProductPres(productService);

        var filterMenu = new FilterMenu(productService, viewProduct);

        var userService = new UserService(databaseService);
        var basketService = new BasketService(databaseService);
        var accountPresentation = new AccountPre(userService);

        var customerMenu = new CustomerMenuPre(viewProduct, filterMenu, accountPresentation);
        var adminMenuPres = new AdminMenuPres(productService, viewProduct, userService);

        var mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres, userService, viewProduct);
        mainMenuPre.Show();
    }
}