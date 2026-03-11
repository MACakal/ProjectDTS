namespace ProjectDTS;

public class MainMenuPre
{
    private CustomerMenuPre _customerMenuPre;
    private AdminMenuPres _adminMenuPres;
    private UserService _userService;
    private ViewProductPres _viewProductPres;
    // private FilterMenu _filterMenu;
    private ProductMenuPres _productMenu;
    public MainMenuPre(CustomerMenuPre customerMenuPre,
     AdminMenuPres adminMenuPres,
      UserService userService,
       ViewProductPres viewProductPres,
       ProductMenuPres productMenu)
    {
        _customerMenuPre = customerMenuPre;
        _adminMenuPres = adminMenuPres;
        _userService = userService;
        _viewProductPres = viewProductPres;
        _productMenu = productMenu;
        // _filterMenu = filterMenu;
    }
    public void Show()
    {
        // DatabaseService db = new();
        // ProductService product = new(db);
        // FilterMenu filter = new(product);
        while (true)
        {
            string[] options =
            {
            "🔍 View products",
            "🔑 Login",
            Console.WriteLine("3. 📝 Register");
            "❌ Exit"
        }
        ;

        int choice = Menu.ShowMenu("===== MAIN MENU =====", options);


        switch (choice)
        {
            case 0:
                Console.Clear();
                // _viewProductPres.Viewproducts();
                // filter.Show();
                _productMenu.Show();
                break;

            case 1:
                Console.Clear();
                Login();
                break;

            case "3":
                Console.Clear();
                Register();
                break;
            case 2:
                Console.Clear();
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
        Console.WriteLine("Enter your email...");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your password...");
        string password = Console.ReadLine();

        var user = _userService.UserLogin(email, password);
        if (user is null)
        {
            Console.WriteLine("Incorrect email or password");
            Console.WriteLine("Please enter any key to return to the main menu.");

            Console.ReadKey();
            Console.Clear();
            return;

        }
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
                break;
            case ProjectDTS.UserRegisterService.emptyParameter:
                System.Console.WriteLine("Please provide valid input");
                break;
            case ProjectDTS.UserRegisterService.UnkownError:
                System.Console.WriteLine("Unkown error.");
                break;
        }


    }
}