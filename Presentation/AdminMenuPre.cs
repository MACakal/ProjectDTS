namespace ProjectDTS;

public class AdminMenuPres
{
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    private ViewProductPres _viewProductPres;
    private AccountPre _accountPre;
    private UserService _userService;
    private readonly GraphInfoAdminPage _graphInfoAdminPage;
    private readonly RoleManagementPres _roleManagementPres;
    private readonly UserActionLogService _userActionLogService;

    public AdminMenuPres(ProductService productService, ViewProductPres viewProductPres, UserService userService,
        RatingService ratingService, Graphservice graphservice, RoleService roleService, UserActionLogService userActionLogService)
    {
        _productService = productService;
        _userService = userService;
        _adminManagerPres = new AdminManagerPres(_userService, roleService);
        _viewProductPres = viewProductPres;
        _accountPre = new(_userService);
        _graphInfoAdminPage = new GraphInfoAdminPage(graphservice);
        _roleManagementPres = new RoleManagementPres(roleService);
        _userActionLogService = userActionLogService;
    }

    public void ShowAdminMenu()
    {
        while (true)
        {
            Console.Clear();
            var user = UserSession.CurrentUser!;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════╗");
            Console.WriteLine("║       Admin Menu       ║");
            Console.WriteLine("╚════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Logged in as: {user.Name} ({user.RoleDisplayName})");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("── Account ───────────────────────");
            Console.ResetColor();
            Console.WriteLine("[1] View Profile");
            Console.WriteLine();

            if (UserSession.Can(Permission.ManageProducts))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── Products ──────────────────────");
                Console.ResetColor();
                Console.WriteLine("[2] View Products");
                Console.WriteLine("[3] Add Product");
                Console.WriteLine("[4] Edit Product");
                Console.WriteLine("[5] Delete Product");
                Console.WriteLine("[6] View Product Logs");
                Console.WriteLine();
            }

            if (UserSession.Can(Permission.ManageOrders))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── Orders ────────────────────────");
                Console.ResetColor();
                Console.WriteLine("[7] Manage Order Status");
                Console.WriteLine();
            }

            if (UserSession.Can(Permission.ViewAnalytics))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── Analytics ─────────────────────");
                Console.ResetColor();
                Console.WriteLine("[8] Most Popular Categories");
                Console.WriteLine("[9] User Spending");
                Console.WriteLine("[10] Notifications");
                Console.WriteLine("[11] Top 3 Products per Category");
                Console.WriteLine("[12] Info from the Graph database");
                Console.WriteLine();
            }

            if (UserSession.Can(Permission.ManageUsers) || UserSession.Can(Permission.ManageReviews))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── Users ─────────────────────────");
                Console.ResetColor();
                if (UserSession.Can(Permission.ManageUsers))
                {
                    Console.WriteLine("[13] View Users");
                    Console.WriteLine("[14] Edit User");
                    Console.WriteLine("[15] Delete User");
                }
                if (UserSession.Can(Permission.ManageReviews))
                    Console.WriteLine("[16] Manage Reviews");
                Console.WriteLine();
            }

            // Manage Roles is SuperAdmin-only (not a general permission)
            if (user.Role == "SuperAdmin")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── Roles ─────────────────────────");
                Console.ResetColor();
                Console.WriteLine("[17] Manage Roles");
                Console.WriteLine();

                // Console.WriteLine("[99] Test Access Denied"); //testing
            }

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
                    _accountPre.AccountInformation(user);
                    break;

                case "2":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageProducts)) { ShowAccessDenied("View Products"); break; }
                    _viewProductPres.BrowseProducts();
                    break;

                case "3":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageProducts)) { ShowAccessDenied("Add Product"); break; }
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
                    if (!UserSession.Can(Permission.ManageProducts)) { ShowAccessDenied("Edit Product"); break; }
                    _adminManagerPres.EditProduct();
                    Console.Clear();
                    break;

                case "5":
                    if (!UserSession.Can(Permission.ManageProducts)) { ShowAccessDenied("Delete Product"); break; }
                    _adminManagerPres.HandleDeleteProduct();
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "6":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageProducts)) { ShowAccessDenied("View Product Logs"); break; }
                    _adminManagerPres.ShowProductLogs();
                    Console.ReadKey();
                    break;

                case "7":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageOrders)) { ShowAccessDenied("Manage Order Status"); break; }
                    _adminManagerPres.ManageOrderStatus();
                    Console.ReadKey();
                    break;

                case "8":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ViewAnalytics)) { ShowAccessDenied("Most Popular Categories"); break; }
                    _adminManagerPres.MostPopularCategories();
                    Console.ReadKey();
                    break;

                case "9":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ViewAnalytics)) { ShowAccessDenied("User Spending"); break; }
                    _adminManagerPres.ShowUserSpending();
                    Console.ReadKey();
                    break;

                case "10":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ViewAnalytics)) { ShowAccessDenied("Notifications"); break; }
                    _adminManagerPres.ShowNotifications();
                    Console.ReadKey();
                    break;

                case "11":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ViewAnalytics)) { ShowAccessDenied("Top Products Per Category"); break; }
                    _adminManagerPres.ShowTopProductsPerCategory();
                    Console.ReadKey();
                    break;

                case "12":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ViewAnalytics)) { ShowAccessDenied("Graph Database Analystics"); break; }
                    _graphInfoAdminPage.Show();
                    break;

                case "13":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageUsers)) { ShowAccessDenied("View Users"); break; }
                    _adminManagerPres.ViewUsers();
                    Console.ReadKey();
                    break;

                case "14":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageUsers)) { ShowAccessDenied("Edit Users"); break; }
                    _adminManagerPres.EditUser();
                    Console.ReadKey();
                    break;

                case "15":
                    Console.Clear();
                    if (!UserSession.Can(Permission.ManageUsers)) { ShowAccessDenied("Delete User"); break; }
                    _adminManagerPres.DeleteUser();
                    Console.ReadKey();
                    break;

                case "16":
                    if (!UserSession.Can(Permission.ManageReviews)) { ShowAccessDenied("Manage Reviews"); break; }
                    _adminManagerPres.HandleDeleteReview();
                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "17":
                    Console.Clear();
                    if (user.Role != "SuperAdmin") { ShowAccessDenied("Manage Roles"); break; }
                    _roleManagementPres.Show();
                    break;
                // case "99":  // testing
                //     ShowAccessDenied("Test Action");
                //     break;

                case "0":
                    Console.Clear();
                    return;
            }
        }
    }

    private void ShowAccessDenied(string attemptedAction)
    {
        // Console.WriteLine("Reached ShowAccessDenied");
        var log = new UserActionLog();
        log.UserId = UserSession.CurrentUser?.Id;
        log.UserSessionId = UserSession.SessionId;
        log.ActionType = "AccessDenied";
        log.Details = new Dictionary<string, string>();
        log.Details.Add("AttemptedAction", attemptedAction);
        // Console.WriteLine("Before Save");
        _userActionLogService.SaveUserActionLogAsync(log);
        // try
        // {
        //     _userActionLogService
        //         .SaveUserActionLogAsync(log)
        //         .GetAwaiter()
        //         .GetResult();

        //     Console.WriteLine("Saved successfully");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"ERROR: {ex.Message}");
        // }
        // Console.WriteLine("After Save");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Access Denied");
        Console.ResetColor();
        Console.ReadKey();
    }
}
