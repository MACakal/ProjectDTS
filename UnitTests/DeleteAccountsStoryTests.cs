using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteAccountsStoryTests
{
    [TestMethod]
    public void DeleteAccounts_ShouldAllowUserAdmin_ToManageUsers()
    {
        var roleService = new RoleService(null!, null!);
        var admin = new User { Role = UserRole.UserAdmin };

        var permissions = roleService.GetPermissionsForUser(admin);

        CollectionAssert.Contains(permissions.ToList(), Permission.ManageUsers);
    }

    [TestMethod]
    public void DeleteAccounts_ShouldNotAllowCustomer_ToManageUsers()
    {
        var roleService = new RoleService(null!, null!);
        var customer = new User { Role = UserRole.Customer };

        var permissions = roleService.GetPermissionsForUser(customer);

        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageUsers);
    }
}
