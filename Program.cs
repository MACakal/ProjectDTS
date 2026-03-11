namespace ProjectDTS;

public class Program
{
    public static void Main(string[] args)
    {
        var databaseService = new DatabaseService();
        var productService = new ProductService(databaseService);
        FilterMenu filterMenu1 = new FilterMenu(productService);

        var viewProduct = new ViewProductPres(productService, filterMenu1);
        var filterMenu = new FilterMenu(productService);
        AdminMenuPres adminMenuPres = new AdminMenuPres(productService, viewProduct);

        var userService = new UserService(databaseService);
        var basketService = new BasketService(databaseService);
        var productMenu = new ProductMenuPres(viewProduct, filterMenu);
        var accountPresentation = new AccountPre(userService);

        var customerMenu = new CustomerMenuPre(viewProduct, filterMenu, accountPresentation);


        MainMenuPre mainMenuPre = new MainMenuPre(customerMenu,
         adminMenuPres,
          userService,
           viewProduct,
           productMenu);
        mainMenuPre.Show();
    }
}