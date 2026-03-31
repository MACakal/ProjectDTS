namespace ProjectDTS;

public class Program
{
    public static void Main(string[] args)
    {
        var databaseService = new DatabaseService();
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