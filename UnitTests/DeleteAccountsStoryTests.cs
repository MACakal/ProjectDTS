using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteAccountsStoryTests
{
    [DataTestMethod]
    [DataRow(UserRole.UserAdmin)]
    [DataRow(UserRole.SuperAdmin)]
    public void DeleteAccounts_ShouldAllowUserAdmins_ToManageUsers(UserRole role)
    {
        var roleService = new RoleService(null!, null!);
        var admin = new User { Role = role };

        var permissions = roleService.GetPermissionsForUser(admin);

        CollectionAssert.Contains(permissions.ToList(), Permission.ManageUsers);
    }

    [DataTestMethod]
    [DataRow(UserRole.Customer)]
    [DataRow(UserRole.ProductAdmin)]
    [DataRow(UserRole.OrderAdmin)]
    public void DeleteAccounts_ShouldNotAllowRolesWithoutUserPermission_ToManageUsers(UserRole role)
    {
        var roleService = new RoleService(null!, null!);
        var user = new User { Role = role };

        var permissions = roleService.GetPermissionsForUser(user);

        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageUsers);
    }
}
