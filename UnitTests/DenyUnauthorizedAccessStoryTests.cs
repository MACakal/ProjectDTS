using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DenyUnauthorizedAccessStoryTests
{
    [TestMethod]
    public void DenyUnauthorizedAccess_ShouldDenyCustomer_AdminPagePermissions()
    {
        var roleService = new RoleService(null!, null!);
        var customer = new User { Role = UserRole.Customer };

        var permissions = roleService.GetPermissionsForUser(customer);

        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageProducts);
        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageOrders);
        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageUsers);
        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ViewAnalytics);
    }

    [TestMethod]
    public void DenyUnauthorizedAccess_ShouldAllowOnlyAssignedPermission()
    {
        UserSession.Clear();
        UserSession.CurrentUser = new User { Id = 1, Role = UserRole.ProductAdmin };
        UserSession.Permissions = new HashSet<Permission> { Permission.ManageProducts };

        Assert.IsTrue(UserSession.Can(Permission.ManageProducts));
        Assert.IsFalse(UserSession.Can(Permission.ManageOrders));
        Assert.IsFalse(UserSession.Can(Permission.ManageUsers));
        Assert.IsFalse(UserSession.Can(Permission.ViewAnalytics));
    }
}
