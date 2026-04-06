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
}