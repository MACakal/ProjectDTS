using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteAccountsStoryTests
{
    [DataTestMethod]
    [DataRow(Permissions.ManageUsers, true)]
    [DataRow(Permissions.ManageProducts, false)]
    [DataRow(Permissions.ManageOrders, false)]
    public void DeleteAccounts_ShouldAllowOnlyRolesWithUserPermission(string permission, bool expected)
    {
        UserSession.Clear();
        UserSession.CurrentUser = new User { Id = 1, Role = "UserAdmin" };
        UserSession.Permissions = new HashSet<string> { Permissions.ManageUsers, Permissions.ManageReviews };

        Assert.AreEqual(expected, UserSession.Can(permission));
    }
}
