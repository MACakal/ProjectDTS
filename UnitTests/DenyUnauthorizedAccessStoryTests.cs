using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DenyUnauthorizedAccessStoryTests
{
    [DataTestMethod]
    [DataRow(Permissions.ManageProducts, true)]
    [DataRow(Permissions.ManageOrders, false)]
    [DataRow(Permissions.ManageUsers, false)]
    [DataRow(Permissions.ViewAnalytics, false)]
    public void DenyUnauthorizedAccess_ShouldAllowOnlyAssignedPermission(string permission, bool expected)
    {
        UserSession.Clear();
        UserSession.CurrentUser = new User { Id = 1, Role = "ProductAdmin" };
        UserSession.Permissions = new HashSet<string> { Permissions.ManageProducts };

        Assert.AreEqual(expected, UserSession.Can(permission));
    }
}
