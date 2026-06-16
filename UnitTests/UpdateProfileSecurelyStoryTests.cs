using ProjectDTS;

namespace UnitTests;

[TestClass]
public class UpdateProfileSecurelyStoryTests
{
    [TestMethod]
    public void UpdateProfileSecurely_ShouldRejectNullUser_WhenCheckingPassword()
    {
        var userService = new UserService(null!);

        var result = userService.UserInformationPasswordCheck(null!, "Password123");

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
            Role = UserRole.SuperAdmin
        };

        var result = userService.UserInformationPasswordCheck(admin, password);

        Assert.AreEqual(UserRegisterService.emptyParameter, result);
    }
}
