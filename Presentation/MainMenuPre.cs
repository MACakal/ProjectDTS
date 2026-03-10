namespace ProjectDTS;

public class MainMenuPre
{
    private CustomerMenuPre _customerMenuPre;
    private AdminMenuPres _adminMenuPres;
    private UserService _userService;
    private ViewProductPres _viewProductPres;
    // private FilterMenu _filterMenu;
    public MainMenuPre(CustomerMenuPre customerMenuPre, AdminMenuPres adminMenuPres, UserService userService, ViewProductPres viewProductPres)
    {
        _customerMenuPre = customerMenuPre;
        _adminMenuPres = adminMenuPres;
        _userService = userService;
        _viewProductPres = viewProductPres;
        // _filterMenu = filterMenu;
    }
    public void Show()
    {
        while (true)
        {
            string[] options =
            {
            "🔍 View products",
            "🔑 Login",
            "❌ Exit"
        };

            int choice = Menu.ShowMenu("===== MAIN MENU =====", options);


            switch (choice)
            {
                case 0:
                    Console.Clear();
                    _viewProductPres.Viewproducts();
                    break;

                case 1:
                    Console.Clear();
                    Login();
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
            _customerMenuPre.CustomerShow();
        }
    }
}
