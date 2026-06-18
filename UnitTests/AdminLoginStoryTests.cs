using ProjectDTS;

namespace UnitTests;

[TestClass]
public class AdminLoginStoryTests
{
    [DataTestMethod]
    [DataRow("ProductAdmin")]
    [DataRow("OrderAdmin")]
    [DataRow("UserAdmin")]
    [DataRow("SuperAdmin")]
    [DataRow("SomeCustomRole")]
    public void AdminLogin_ShouldRecognizeAdminRoles_AsAdmin(string role)
    {
        var user = new User { Role = role };
        Assert.IsTrue(user.IsAdmin());
    }

    [DataTestMethod]
    [DataRow("Customer")]
    public void AdminLogin_ShouldNotTreatCustomer_AsAdmin(string role)
    {
        var user = new User { Role = role };
        Assert.IsFalse(user.IsAdmin());
    }
}
