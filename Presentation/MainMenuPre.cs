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
            Console.WriteLine("4. 🔓 Forgot password?");
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
                case "4":
                    Console.Clear();
                    ForgotPassword();
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
                    SetSecurityQuestionPrompt(loggedInUser);
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

    public void SetSecurityQuestionPrompt(User user)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Set a security question to recover your account if you forget your password.");
        Console.ResetColor();

        string[] questions = {
            "What was the name of your first pet?",
            "What is your mother's maiden name?",
            "What was the name of your elementary school?",
            "What city were you born in?",
            "What is your favorite movie?"
        };

        Console.WriteLine("Choose a security question:");
        for (int i = 0; i < questions.Length; i++)
            Console.WriteLine($"{i + 1}. {questions[i]}");

        string question = null;
        while (question == null)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int q) && q >= 1 && q <= questions.Length)
                question = questions[q - 1];
            else
                Console.WriteLine("Invalid choice, try again:");
        }

        Console.Write("Your answer: ");
        string answer = Console.ReadLine()!;

        _userService.SetSecurityQuestion(user.Id, question, answer);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Security question saved.");
        Console.ResetColor();
    }

    private void ForgotPassword()
    {
        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine()!;

        string? question = _userService.GetSecurityQuestion(email);

        if (question == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No security question found for this account.");
            Console.WriteLine("If your account was created before this feature existed, log in and set one from Account Settings.");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Security question: {question}");
        Console.Write("Your answer: ");
        string answer = Console.ReadLine()!;

        Console.WriteLine("Enter your new password: (At least 6 chars, 1 uppercase, 1 number)");
        string newPassword = Console.ReadLine()!;
        while (!userLogic.CheckPassword(newPassword))
        {
            Console.WriteLine("Invalid password. Try again:");
            newPassword = Console.ReadLine()!;
        }

        var result = _userService.ResetPasswordWithSecurityAnswer(email, answer, newPassword);

        if (result == UserRegisterService.succesfull)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Password reset successfully! You can now log in.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Incorrect answer. Password was not reset.");
        }

        Console.ResetColor();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}