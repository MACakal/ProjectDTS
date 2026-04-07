namespace ProjectDTS;

public class AccountPre
{
    private UserService _userService {get;set;}

    public AccountPre(UserService service)
    {
        _userService = service;
    }

    public void AccountInformation(User user)
    {
        BreadcrumbManager.Push("Account Information");
        BreadcrumbManager.Render();
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
        System.Console.WriteLine();
        System.Console.WriteLine("Click enter to continue....");
        Console.ReadKey();
        BreadcrumbManager.Pop();
    }
}