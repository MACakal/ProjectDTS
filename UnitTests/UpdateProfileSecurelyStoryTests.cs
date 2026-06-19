using ProjectDTS;

namespace UnitTests;

[TestClass]
public class UpdateProfileSecurelyStoryTests
{
    [DataTestMethod]
    [DataRow("Password123")]
    [DataRow("x")]
    [DataRow("")]
    public void UpdateProfileSecurely_ShouldRejectNullUser_WhenCheckingPassword(string password)
    {
        var userService = new UserService(null!);

        var result = userService.UserInformationPasswordCheck(null!, password);

        Assert.AreEqual(UserRegisterService.emptyParameter, result);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public void UpdateProfileSecurely_ShouldRejectEmptyPassword_WhenCheckingPassword(string password)
    {
        var userService = new UserService(null!);
        var admin = new User
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@example.com",
            Role = "SuperAdmin"
        };

        var result = userService.UserInformationPasswordCheck(admin, password);

        Assert.AreEqual(UserRegisterService.emptyParameter, result);
    }
}
