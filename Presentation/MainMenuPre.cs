namespace ProjectDTS;

public class MainMenuPre
{
    private CustomerMenuPre _customerMenuPre;
    private AdminMenuPres _adminMenuPres;
    private UserService _userService;
    private ViewProductPres _viewProductPres;
    // private FilterMenu _filterMenu;
    // private ProductMenuPres _productMenu;
    public MainMenuPre(CustomerMenuPre customerMenuPre,
     AdminMenuPres adminMenuPres,
      UserService userService,
       ViewProductPres viewProductPres
       )
    {
        _customerMenuPre = customerMenuPre;
        _adminMenuPres = adminMenuPres;
        _userService = userService;
        _viewProductPres = viewProductPres;
        // _productMenu = productMenu;
        // _filterMenu = filterMenu;
    }
    public void Show()
    {
        // DatabaseService db = new();
        // ProductService product = new(db);
        // FilterMenu filter = new(product);
        BreadcrumbManager.Push("Main Menu");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== MAIN MENU =====");
            Console.WriteLine("1. 🔍 View products");
            Console.WriteLine("2. 🔑 Login");
            Console.WriteLine("3. 📝 Register");
            Console.WriteLine("0. ❌ Exit");
            //     string[] options =
            //     {
            //     "🔍 View products",
            //     "🔑 Login",
            //     "📝 Register",
            //     "❌ Exit"
            // };


            string choice = Console.ReadLine();


            switch (choice)
            {

                case "1":
                    Console.Clear();
                    BreadcrumbManager.Push("View Products");
                    _viewProductPres.Viewproducts();
                    // filter.Show();
                    // Console.CursorVisible = true;
                    // _viewProductPres.Viewproducts();
                    break;

                case "2":
                    Console.Clear();
                    BreadcrumbManager.Push("Login");
                    // Console.CursorVisible = true;
                    Login();
                    break;

                case "3":
                    BreadcrumbManager.Push("Register");
                    Console.Clear();
                    // Console.CursorVisible = true;
                    Register();
                    break;
                case "0":
                    Console.Clear();
                    // Console.CursorVisible = true;
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    Console.ReadKey();
                    break;
            }
        }
    }


    private void Login()
    {
        BreadcrumbManager.Render();
        Console.WriteLine("Enter your email...");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your password...");
        string password = Password.ReadPassword();

        var user = _userService.UserLogin(email, password);
        if (user is null)
        {
            Console.WriteLine("\nIncorrect email or password");
            Console.WriteLine("Please enter any key to return to the main menu.");

            Console.ReadKey();
            Console.Clear();
            BreadcrumbManager.Pop();
            return;

        }
        UserSession.CurrentUser = user;
        if (user.Role == UserRole.Admin)
        {
            Console.Clear();
            BreadcrumbManager.Pop();
            BreadcrumbManager.Push("Admin Menu");
            _adminMenuPres.ShowAdminMenu();
        }
        else
        {
            BreadcrumbManager.Push("shopping Menu");
            _customerMenuPre.CustomerShow(user);
        }
    }

    private void Register()
    {
        BreadcrumbManager.Render();
        System.Console.WriteLine("Enter your name...");
        string name = Console.ReadLine();
        System.Console.WriteLine("Enter your email adress...");
        string email = Console.ReadLine();
        System.Console.WriteLine("Enter your password...");
        string password = Console.ReadLine();

        ProjectDTS.UserRegisterService outcome = _userService.UserRegister(name: name, email: email, password: password);

        switch (outcome)
        {
            case ProjectDTS.UserRegisterService.succesfull:
                System.Console.WriteLine("Your account is succesfully created.");
                var loggedInUser = _userService.UserLogin(email, password);
                if (loggedInUser != null)
                {
                    UserSession.CurrentUser = loggedInUser;
                    System.Console.WriteLine($"Welcome, {loggedInUser.Name}! You are now logged in.");
                }
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                BreadcrumbManager.Pop();
                break;
            // break;
            case ProjectDTS.UserRegisterService.emptyParameter:
                System.Console.WriteLine("Please provide valid input");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                BreadcrumbManager.Pop();
                break;
            case ProjectDTS.UserRegisterService.alreadyExists:
                System.Console.WriteLine("An account with this email already exists");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                BreadcrumbManager.Pop();
                break;
            case ProjectDTS.UserRegisterService.UnkownError:
                System.Console.WriteLine("Unkown error.");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                BreadcrumbManager.Pop();
                break;
        }


    }
}