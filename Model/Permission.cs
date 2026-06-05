namespace ProjectDTS;

public enum Permission
{
    ManageProducts,
    ManageOrders,
    ManageUsers,
    ManageReviews,
    ViewAnalytics,
    AssignRoles
}

public static class PermissionDescriptions
{
    public static string Describe(Permission p) => p switch
    {
        Permission.ManageProducts => "Manage Products & Product Logs (options 2-6)",
        Permission.ManageOrders   => "Manage Order Status (option 7)",
        Permission.ViewAnalytics  => "View Analytics & Notifications (options 8-12)",
        Permission.ManageUsers    => "View/Edit/Delete Users (options 13-15)",
        Permission.ManageReviews  => "Manage Reviews (option 16)",
        Permission.AssignRoles    => "Assign/Change User Roles",
        _                         => p.ToString()
    };

    public static readonly Permission[] All = Enum.GetValues<Permission>();
}
