using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteProductsStoryTests
{
    [DataTestMethod]
    [DataRow(UserRole.ProductAdmin)]
    [DataRow(UserRole.SuperAdmin)]
    public void DeleteProducts_ShouldAllowProductAdmins_ToManageProducts(UserRole role)
    {
        var roleService = new RoleService(null!, null!);
        var admin = new User { Role = role };

        var permissions = roleService.GetPermissionsForUser(admin);

        CollectionAssert.Contains(permissions.ToList(), Permission.ManageProducts);
    }

    [DataTestMethod]
    [DataRow(UserRole.Customer)]
    [DataRow(UserRole.OrderAdmin)]
    [DataRow(UserRole.UserAdmin)]
    public void DeleteProducts_ShouldNotAllowRolesWithoutProductPermission_ToManageProducts(UserRole role)
    {
        var roleService = new RoleService(null!, null!);
        var user = new User { Role = role };

        var permissions = roleService.GetPermissionsForUser(user);

        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageProducts);
    }
}
