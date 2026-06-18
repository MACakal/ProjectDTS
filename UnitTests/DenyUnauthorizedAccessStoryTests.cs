using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DenyUnauthorizedAccessStoryTests
{
    [DataTestMethod]
    [DataRow(UserRole.Customer, Permission.ManageProducts)]
    [DataRow(UserRole.Customer, Permission.ManageOrders)]
    [DataRow(UserRole.Customer, Permission.ManageUsers)]
    [DataRow(UserRole.Customer, Permission.ViewAnalytics)]
    [DataRow(UserRole.ProductAdmin, Permission.ManageOrders)]
    [DataRow(UserRole.OrderAdmin, Permission.ManageProducts)]
    public void DenyUnauthorizedAccess_ShouldDenyRolesWithoutPermission(UserRole role, Permission deniedPermission)
    {
        var roleService = new RoleService(null!, null!);
        var user = new User { Role = role };

        var permissions = roleService.GetPermissionsForUser(user);

        CollectionAssert.DoesNotContain(permissions.ToList(), deniedPermission);
    }

    [DataTestMethod]
    [DataRow(Permission.ManageProducts, true)]
    [DataRow(Permission.ManageOrders, false)]
    [DataRow(Permission.ManageUsers, false)]
    [DataRow(Permission.ViewAnalytics, false)]
    public void DenyUnauthorizedAccess_ShouldAllowOnlyAssignedPermission(Permission permission, bool expected)
    {
        UserSession.Clear();
        UserSession.CurrentUser = new User { Id = 1, Role = UserRole.ProductAdmin };
        UserSession.Permissions = new HashSet<Permission> { Permission.ManageProducts };

        Assert.AreEqual(expected, UserSession.Can(permission));
    }
}
