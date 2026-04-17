using ProjectDTS;

public class UserLogic
{
    private static UserService _userservice = new UserService(new DatabaseService());

    public UserRegisterService UpdateUser(User user)
    {
        if (user == null)
        {
            return UserRegisterService.emptyParameter;
        }

        return _userservice.UpdateUser(user);
    }

    public UserRegisterService DeleteUser(User user)
    {
        if (user == null)
        {
            return UserRegisterService.emptyParameter;
        }

        return _userservice.DeleteUser(user.Id);
    }

    public bool CheckPassword(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }
        if (!password.Any(ch => char.IsUpper(ch)))
        {
            return false;
        }
        if (!password.Any(ch => char.IsDigit(ch)))
        {
            return false;
        }
        return true;
    }

    public bool CheckEmailCorrect(string email)
    {
        return email.Contains("@");
    }

}