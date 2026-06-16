using ProjectDTS;

namespace UnitTests;

[TestClass]
public class DeleteProductsStoryTests
{
    [TestMethod]
    public void DeleteProducts_ShouldAllowProductAdmin_ToManageProducts()
    {
        var roleService = new RoleService(null!, null!);
        var admin = new User { Role = UserRole.ProductAdmin };

        var permissions = roleService.GetPermissionsForUser(admin);

        CollectionAssert.Contains(permissions.ToList(), Permission.ManageProducts);
    }

    [TestMethod]
    public void DeleteProducts_ShouldNotAllowCustomer_ToManageProducts()
    {
        var roleService = new RoleService(null!, null!);
        var customer = new User { Role = UserRole.Customer };

        var permissions = roleService.GetPermissionsForUser(customer);

        CollectionAssert.DoesNotContain(permissions.ToList(), Permission.ManageProducts);
    }
}
