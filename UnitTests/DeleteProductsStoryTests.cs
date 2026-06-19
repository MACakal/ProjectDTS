using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteProductsStoryTests
{
    [DataTestMethod]
    [DataRow(Permissions.ManageProducts, true)]
    [DataRow(Permissions.ManageOrders, false)]
    [DataRow(Permissions.ManageUsers, false)]
    public void DeleteProducts_ShouldAllowOnlyRolesWithProductPermission(string permission, bool expected)
    {
        UserSession.Clear();
        UserSession.CurrentUser = new User { Id = 1, Role = "ProductAdmin" };
        UserSession.Permissions = new HashSet<string> { Permissions.ManageProducts };

        Assert.AreEqual(expected, UserSession.Can(permission));
    }
}
