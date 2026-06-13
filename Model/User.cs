namespace ProjectDTS;

public enum UserRole
{
    Customer,
    ProductAdmin,
    OrderAdmin,
    UserAdmin,
    SuperAdmin,
    Custom  // any role defined dynamically in the roles table
}

public static class UserRoleExtensions
{
    public static bool IsAdmin(this UserRole role) => role != UserRole.Customer;
}

public class User
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    public string? SecurityQuestion { get; set; }
    public string? SecurityAnswer { get; set; }

    public UserRole Role { get; set; }

    // Only populated when Role == UserRole.Custom
    public string? CustomRoleName { get; set; }

    // Returns the display name of the role (custom role name or enum name)
    public string RoleDisplayName => Role == UserRole.Custom ? (CustomRoleName ?? "Custom") : Role.ToString();

    public User() { }
    public User(string name, string email, string password, string address = null, string zipCode = null, string country = null)
    {
        Name = name;
        Email = email;
        Password = password;
        Address = address;
        ZipCode = zipCode;
        Country = country;
        Role = UserRole.Customer;
    }
}
