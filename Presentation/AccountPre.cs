namespace ProjectDTS;

public class AccountPre
{
    private UserService _userService {get;set;}
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

        switch(result)
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
        Console.Clear();
        System.Console.WriteLine("===Account information===");
        System.Console.WriteLine($"Account email: {user.Email}");
        System.Console.WriteLine($"Account name: {user.Name}");
        System.Console.WriteLine($"Account password: {user.Password}");
        System.Console.WriteLine($"Account role: {user.Role}");
        Console.WriteLine("\n===Account options===");
        Console.WriteLine("[1] Change account information");
        Console.WriteLine("[2] DELETE account");
        Console.WriteLine("[3] Go back to user menu");
        string choice = Console.ReadLine()!;

        if(choice == "1")
        {
            ChangeAccInfo(user);
        }
        else if(choice == "2")
        {
            DeleteAcc(user);
        }
        else if(choice == "3")
        {
            return;
        }
        else
        {
            SuccesfullCheck(user);
        }

    }

    public void ChangeAccInfo(User user)
    {
        Console.Clear();
        Console.WriteLine("=== Change Account Information ===");
        Console.WriteLine("Press ENTER to skip.\n");

        Console.WriteLine($"Current name: {user.Name}");
        Console.Write("New name: ");
        var inputName = Console.ReadLine();

        if (inputName != null && inputName != "")
        {
            user.Name = inputName;
        }

        Console.WriteLine($"\nCurrent email: {user.Email}");
        Console.Write("New email: ");
        var inputEmail = Console.ReadLine();

        if (inputEmail != null && inputEmail != "")
        {
            user.Email = inputEmail;
        }

        Console.WriteLine($"\nCurrent password: {user.Password}");
        Console.Write("New password: ");
        var inputPassword = Console.ReadLine();

        if (inputPassword != null && inputPassword != "")
        {
            user.Password = inputPassword;
        }

        var result = userLogic.UpdateUser(user);

        if (result == UserRegisterService.succesfull)
        {
            Console.WriteLine("\nAccount updated! (Press ENTER to continue...)");
        }
        else
        {
            Console.WriteLine("\nSomething went wrong... (Press ENTER to continue...)");
        }

        Console.ReadLine();
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
    }
}