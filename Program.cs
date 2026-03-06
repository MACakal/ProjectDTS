namespace ProjectDTS;

public class Program
{
    public static void Main(string[] args)
    {
        var databaseService = new DatabaseService();
        var productService = new ProductService(databaseService);
        var filterMenu = new FilterMenu(productService);

        var customerMenu = new CustomerMenuPre(productService, filterMenu);
        AdminMenuPres adminMenuPres = new AdminMenuPres(productService);

        MainMenuPre mainMenuPre = new MainMenuPre(customerMenu, adminMenuPres);
        mainMenuPre.Show();

        // customerMenu.CustomerShow();
        // adminMenuPres.ShowAdminMenu();
        // var databaseService = new DatabaseService();
        // var productService = new ProductService(databaseService);
    }
}