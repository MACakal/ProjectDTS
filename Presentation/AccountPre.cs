namespace ProjectDTS;

public class AccountPre
{
    private UserService _userService { get; set; }
    static private UserLogic userLogic = new();

    public AccountPre(UserService service)
    {
        _userService = service;
    }

    public void AccountInformation(User user)
    {

        System.Console.WriteLine("Please enter your password to see your profile information!");
        var password = Console.ReadLine();
        var result = _userService.UserInformationPasswordCheck(user, password);

        switch (result)
        {
            case ProjectDTS.UserRegisterService.succesfull:
                SuccesfullCheck(user);
                break;
            case ProjectDTS.UserRegisterService.emptyParameter:
                System.Console.WriteLine($"Please provide correct input! (Press enter to continue)");
                Console.ReadKey();
                break;
            case ProjectDTS.UserRegisterService.UnkownError:
                System.Console.WriteLine($"Unkown error has occured. (Press enter to continue)");
                Console.ReadKey();
                break;
        }
    }

    public void SuccesfullCheck(User user)
    {
        while (true)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================");
            Console.WriteLine("        ACCOUNT INFORMATION       ");
            Console.WriteLine("==================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Email: ");
            Console.ResetColor();
            Console.WriteLine(user.Email);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Name : ");
            Console.ResetColor();
            Console.WriteLine(user.Name);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Password: ");
            Console.ResetColor();
            Console.WriteLine("********");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Role : ");
            Console.ResetColor();
            Console.WriteLine(user.Role);

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("========== OPTIONS ==========");
            Console.ResetColor();

            Console.WriteLine("[1] Change account information");
            Console.WriteLine("[2] Delete account");
            Console.WriteLine("[3] Set/change security question");
            Console.WriteLine("[0] Back");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Select option: ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ChangeAccInfo(user);
                    break;

                case "2":
                    DeleteAcc(user);
                    return;

                case "3":
                    SetSecurityQuestion(user);
                    break;

                case "0":
                    return;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid option...");
                    Console.ResetColor();
                    Console.ReadKey();
                    break;
            }
        }
    }
    public void ChangeAccInfo(User user)
    {
        bool editing = true;

        while (editing)
        {
            Console.Clear();


            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════╗");
            Console.WriteLine("║       EDIT YOUR PROFILE      ║");
            Console.WriteLine("╚══════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine();

            Console.WriteLine($"1. Name     : {user.Name}");
            Console.WriteLine($"2. Email    : {user.Email}");
            Console.WriteLine($"3. Password : ********");
            Console.WriteLine();
            Console.WriteLine("4. Save and exit");
            Console.WriteLine("5. Cancel");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Select option: ");
            Console.ResetColor();

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter new name: ");
                    var name = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(name))
                        user.Name = name;
                    break;

                case "2":
                    Console.Write("Enter new email: ");
                    var email = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(email))
                        user.Email = email;
                    break;

                case "3":
                    Console.Write("Enter new password: ");
                    var password = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(password))
                        user.Password = password;
                    break;

                case "4":
                    var result = userLogic.UpdateUser(user);

                    Console.Clear();

                    if (result == UserRegisterService.succesfull)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✔ Account updated successfully!");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("✖ Something went wrong.");
                    }

                    Console.ResetColor();
                    Console.WriteLine("\nPress any key...");
                    Console.ReadKey();

                    editing = false;
                    break;

                case "5":
                    Console.WriteLine("Changes discarded.");
                    Thread.Sleep(800);
                    editing = false;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option...");
                    Console.ResetColor();
                    Thread.Sleep(800);
                    break;
            }
        }

        SuccesfullCheck(user);
    }


    public void SetSecurityQuestion(User user)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=== SET SECURITY QUESTION ===");
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

        var result = _userService.SetSecurityQuestion(user.Id, question, answer);

        if (result == UserRegisterService.succesfull)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Security question saved successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Something went wrong. Please try again.");
        }

        Console.ResetColor();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public void DeleteAcc(User user)
    {
        Console.Clear();
        Console.WriteLine("Are you sure you want to delete your account? (y/n)");

        var input = Console.ReadLine();

        if (input != null && input.ToLower() == "y")
        {
            var result = userLogic.DeleteUser(user);

            if (result == UserRegisterService.succesfull)
            {
                UserSession.CurrentUser = null;
                Console.WriteLine("Account deleted. (Press ENTER to continue...)");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine("Error deleting account. (Press ENTER to continue...)");
                Console.ReadLine();
                SuccesfullCheck(user);
            }
        }
        else
        {
            Console.WriteLine("Cancelled. (Press ENTER to continue...)");
            Console.ReadLine();
            SuccesfullCheck(user);
        }
        //BreadcrumbManager.Pop();
    }
}