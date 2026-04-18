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
        //BreadcrumbManager.Push("Account Information");
        //BreadcrumbManager.Render();
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

    // public void SuccesfullCheck(User user)
    // {
    //     Console.Clear();
    //     System.Console.WriteLine("===Account information===");
    //     System.Console.WriteLine($"Account email: {user.Email}");
    //     System.Console.WriteLine($"Account name: {user.Name}");
    //     System.Console.WriteLine($"Account password: *********");
    //     System.Console.WriteLine($"Account role: {user.Role}");
    //     Console.WriteLine("\n===Account options===");
    //     Console.WriteLine("[1] Change account information");
    //     Console.WriteLine("[2] DELETE account");
    //     Console.WriteLine("[3] Go back to user menu");
    //     string choice = Console.ReadLine()!;

    //     if (choice == "1")
    //     {
    //         ChangeAccInfo(user);
    //     }
    //     else if (choice == "2")
    //     {
    //         DeleteAcc(user);
    //     }
    //     else if (choice == "3")
    //     {
    //         return;
    //     }
    //     else
    //     {
    //         SuccesfullCheck(user);
    //     }

    // }
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