using ProjectDTS;

namespace UnitTests;

[TestClass]
public class AdminLoginStoryTests
{
    [DataTestMethod]
    [DataRow(UserRole.ProductAdmin)]
    [DataRow(UserRole.OrderAdmin)]
    [DataRow(UserRole.UserAdmin)]
    [DataRow(UserRole.SuperAdmin)]
    [DataRow(UserRole.Custom)]
    public void AdminLogin_ShouldRecognizeAdminRoles_AsAdmin(UserRole role)
    {
        Assert.IsTrue(role.IsAdmin());
    }

    [DataTestMethod]
    [DataRow(UserRole.Customer)]
    public void AdminLogin_ShouldNotTreatCustomer_AsAdmin(UserRole role)
    {
        Assert.IsFalse(role.IsAdmin());
    }
}
