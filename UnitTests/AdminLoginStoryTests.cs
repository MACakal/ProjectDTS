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
    public void AdminLogin_ShouldRecognizeAdminRoles_AsAdmin(UserRole role)
    {
        Assert.IsTrue(role.IsAdmin());
    }

    [TestMethod]
    public void AdminLogin_ShouldNotTreatCustomer_AsAdmin()
    {
        Assert.IsFalse(UserRole.Customer.IsAdmin());
    }
}
