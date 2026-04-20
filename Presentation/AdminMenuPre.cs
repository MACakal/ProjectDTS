
namespace ProjectDTS;

public class AdminMenuPres
{
    // DatabaseService db = new DatabaseService();
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    private ViewProductPres _viewProductPres;
    private AccountPre _accountPre;
    private UserService _userService;
    public AdminMenuPres(ProductService productService, ViewProductPres viewProductPres, UserService userService)
    {
        _productService = productService;
        _userService = userService;
        _adminManagerPres = new AdminManagerPres(_userService);
        _viewProductPres = viewProductPres;
        _accountPre = new(_userService);
    }


    public void ShowAdminMenu()
    {
        while (true)
        {
            Console.Clear();
            var user = UserSession.CurrentUser;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════╗");
            Console.WriteLine("║       Admin Menu       ║");
            Console.WriteLine("╚════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Logged in as: {user?.Name} ({user?.Role})");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("── Account ───────────────────────");
            Console.ResetColor();
            Console.WriteLine("[1] View Profile");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("── Products ──────────────────────");
            Console.ResetColor();
            Console.WriteLine("[2] View Products");
            Console.WriteLine("[3] Add Product");
            Console.WriteLine("[4] Edit Product");
            Console.WriteLine("[5] Delete Product");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("── Analytics ─────────────────────");
            Console.ResetColor();
            Console.WriteLine("[6] Most Popular Categories");
            Console.WriteLine("[7] User Spending");
            Console.WriteLine("[8] Notifications");
            Console.WriteLine("[9] Top 3 Products per Category");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("── Users ─────────────────────────");
            Console.ResetColor();
            Console.WriteLine("[10] View Users");
            Console.WriteLine("[11] Edit User");
            Console.WriteLine("[12] Delete User");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[0] Back");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Select option: ");
            Console.ResetColor();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();

                    if (user is null || user.Role != UserRole.Admin)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Access denied.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                    }
                    _accountPre.AccountInformation(user);
                    break;
                case "2":
                    Console.Clear();
                    _viewProductPres.Viewproducts();
                    break;

                case "3":
                    Console.Clear();

                    Console.WriteLine("Add product...\n");
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
                case "4":
                    Console.Clear();
                    _adminManagerPres.EditProduct();
                    Console.Clear();
                    break;
                case "5":
                    _adminManagerPres.HandleDeleteProduct();
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "6":
                    Console.Clear();
                    _adminManagerPres.MostPopularCategories();
                    Console.ReadKey();
                    break;

                case "7":
                    Console.Clear();
                    _adminManagerPres.ShowUserSpending();
                    Console.ReadKey();
                    break;

                case "8":
                    Console.Clear();
                    _adminManagerPres.ShowNotifications();
                    Console.ReadKey();
                    break;

                case "9":
                    Console.Clear();
                    _adminManagerPres.ShowTopProductsPerCategory();
                    Console.ReadKey();
                    break;
                case "10":
                    Console.Clear();
                    _adminManagerPres.ViewUsers();
                    Console.ReadKey();
                    break;

                case "11":
                    Console.Clear();
                    _adminManagerPres.EditUser();
                    Console.ReadKey();
                    break;

                case "12":
                    Console.Clear();
                    _adminManagerPres.DeleteUser();
                    Console.ReadKey();
                    break;
                case "0":
                    Console.Clear();
                    return;
            }
        }
    }
}
