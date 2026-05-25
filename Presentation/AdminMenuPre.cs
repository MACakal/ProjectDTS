namespace ProjectDTS;

public class AdminMenuPres
{
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    private ViewProductPres _viewProductPres;
    private AccountPre _accountPre;
    private UserService _userService;
    private readonly GraphInfoAdminPage _graphInfoAdminPage;

    public AdminMenuPres(ProductService productService, ViewProductPres viewProductPres, UserService userService, RatingService ratingService, Graphservice graphservice)
    {
        _productService = productService;
        _userService = userService;
        _adminManagerPres = new AdminManagerPres(_userService);
        _viewProductPres = viewProductPres;
        _accountPre = new(_userService);
        _graphInfoAdminPage = new GraphInfoAdminPage(graphservice);
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
            Console.WriteLine("[6] View Product Logs");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("── Orders ────────────────────────");
            Console.ResetColor();
            Console.WriteLine("[7] Manage Order Status");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("── Analytics ─────────────────────");
            Console.ResetColor();
            Console.WriteLine("[8] Most Popular Categories");
            Console.WriteLine("[9] User Spending");
            Console.WriteLine("[10] Notifications");
            Console.WriteLine("[11] Top 3 Products per Category");
            Console.WriteLine("[12] Info from the Graph database");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("── Users ─────────────────────────");
            Console.ResetColor();
            Console.WriteLine("[13] View Users");
            Console.WriteLine("[14] Edit User");
            Console.WriteLine("[15] Delete User");
            Console.WriteLine("[16] Manage Reviews");
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
                    _adminManagerPres.ShowProductLogs();
                    Console.ReadKey();
                    break;
                case "7":
                    Console.Clear();
                    _adminManagerPres.ManageOrderStatus();
                    Console.ReadKey();
                    break;
                case "8":
                    Console.Clear();
                    _adminManagerPres.MostPopularCategories();
                    Console.ReadKey();
                    break;
                case "9":
                    Console.Clear();
                    _adminManagerPres.ShowUserSpending();
                    Console.ReadKey();
                    break;
                case "10":
                    Console.Clear();
                    _adminManagerPres.ShowNotifications();
                    Console.ReadKey();
                    break;
                case "11":
                    Console.Clear();
                    _adminManagerPres.ShowTopProductsPerCategory();
                    Console.ReadKey();
                    break;
                case "12":
                    Console.Clear();
                    _graphInfoAdminPage.Show();
                    break;
                case "13":
                    Console.Clear();
                    _adminManagerPres.ViewUsers();
                    Console.ReadKey();
                    break;
                case "14":
                    Console.Clear();
                    _adminManagerPres.EditUser();
                    Console.ReadKey();
                    break;
                case "15":
                    Console.Clear();
                    _adminManagerPres.DeleteUser();
                    Console.ReadKey();
                    break;
                case "16":
                    _adminManagerPres.HandleDeleteReview();
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