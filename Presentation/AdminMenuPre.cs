
namespace ProjectDTS;

public class AdminMenuPres
{
    // DatabaseService db = new DatabaseService();
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    private ViewProductPres _viewProductPres;

    private UserService _userService;
    public AdminMenuPres(ProductService productService, ViewProductPres viewProductPres, UserService userService)
    {
        _productService = productService;
        _userService = userService;
        _adminManagerPres = new AdminManagerPres(_userService);
        _viewProductPres = viewProductPres;
    }


    public void ShowAdminMenu()
    {
        while (true)
        {
            Console.WriteLine(" ====Admin Menu==== ");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add Product");
            Console.WriteLine("3. Edit Product");
            Console.WriteLine("4. Delete Product");
            Console.WriteLine("5. Analytics");
            Console.WriteLine("6. User Spending");
            Console.WriteLine("0. Back");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    _viewProductPres.Viewproducts();
                    break;

                case "2":
                    Console.Clear();

                    Console.WriteLine("Add product...");
                    var product = _adminManagerPres.CreateProduct();
                    if (product == null) return;
                    _productService.AddProduct(product);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Product added successfully.");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Press any key...");
                    Console.ResetColor();

                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "3":
                    _adminManagerPres.EditProduct();
                    Console.Clear();
                    break;
                case "4":
                    _adminManagerPres.HandleDeleteProduct();
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "5":
                    _adminManagerPres.MostPopularCategories();
                    Console.Clear();
                    break;
                case "6":
                    Console.Clear();
                    _adminManagerPres.ShowUserSpending();
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "0":
                    Console.Clear();
                    return;
            }
        }
    }
}
