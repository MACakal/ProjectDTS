namespace ProjectDTS;

public class Program
{
    public static void Main(string[] args)
    {
        var databaseService = new DatabaseService();
        var productService = new ProductService(databaseService);
        var viewProduct = new ViewProductPres(productService);

        // var customerMenu = new CustomerMenuPre(productService);
        var customerMenu = new CustomerMenuPre(viewProduct);
        AdminMenuPres adminMenuPres = new AdminMenuPres(productService, viewProduct);

        var userService = new UserService(databaseService);

        MainMenuPre mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres, userService, viewProduct);
        mainMenuPre.Show();

        // customerMenu.CustomerShow();
        // adminMenuPres.ShowAdminMenu();
        // var databaseService = new DatabaseService();
        // var productService = new ProductService(databaseService);
    }
}