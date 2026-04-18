namespace ProjectDTS;

public class MainMenuPre
{
    static private UserLogic userLogic = new();
    private CustomerMenuPre _customerMenuPre;
    private AdminMenuPres _adminMenuPres;
    private UserService _userService;
    private ViewProductPres _viewProductPres;

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

    }
    public void Show()
    {


        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("╔════════════════════════════╗");

            string title = "WELCOME TO THE WEBSHOP";
            ConsoleColor[] colors = {

            ConsoleColor.Red,

            ConsoleColor.Yellow,

            ConsoleColor.Green,

            ConsoleColor.Cyan,

            ConsoleColor.Blue,

            ConsoleColor.Magenta

            };

            Console.Write("║   ");
            for (int i = 0; i < title.Length; i++)
            {
                Console.ForegroundColor = colors[i % colors.Length];
                Console.Write(title[i]);
            }
            Console.WriteLine("   ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚════════════════════════════╝");

            Console.ResetColor();
            // Console.WriteLine("===== MAIN MENU =====");
            Console.WriteLine("1. 🔍 View products");
            Console.WriteLine("2. 🔑 Login");
            Console.WriteLine("3. 📝 Register");
            Console.WriteLine("0. ❌ Exit");



            string choice = Console.ReadLine();


            switch (choice)
            {

                case "1":
                    Console.Clear();
                    _viewProductPres.Viewproducts();

                    break;

                case "2":
                    Console.Clear();
                    Login();
                    break;

                case "3":
                    Console.Clear();
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
        //  BreadcrumbManager.Render();
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
            //BreadcrumbManager.Pop();
            return;

        }
        UserSession.CurrentUser = user;
        if (user.Role == UserRole.Admin)
        {
            Console.Clear();

            _adminMenuPres.ShowAdminMenu();
        }
        else
        {
            _customerMenuPre.CustomerShow(user);
        }
    }

    private void Register()
    {
        System.Console.WriteLine("Enter your name...");
        string name = Console.ReadLine()!;
        System.Console.WriteLine("Enter your email adress... (Has to contain a @)");
        string email = Console.ReadLine()!;

        while (!userLogic.CheckEmailCorrect(email))
        {
            Console.WriteLine("Email must contain a @. Try again:");
            email = Console.ReadLine()!;
        }

        Console.WriteLine("Enter your password... (At least 6 chars long, 1 Uppercase letter and 1 number)");
        string password = Console.ReadLine()!;

        while (!userLogic.CheckPassword(password))
        {
            Console.WriteLine("Password must be at least 6 characters, contain 1 Uppercase letter and 1 number. Try again:");
            password = Console.ReadLine()!;
        }

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
                //BreadcrumbManager.Pop();
                break;
            // break;
            case ProjectDTS.UserRegisterService.emptyParameter:
                System.Console.WriteLine("Please provide valid input");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                //BreadcrumbManager.Pop();
                break;
            case ProjectDTS.UserRegisterService.alreadyExists:
                System.Console.WriteLine("An account with this email already exists");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                //BreadcrumbManager.Pop();
                break;
            case ProjectDTS.UserRegisterService.UnkownError:
                System.Console.WriteLine("Unkown error.");
                System.Console.WriteLine("Press any key to continue");
                System.Console.ReadKey();
                //BreadcrumbManager.Pop();
                break;
        }


    }
}