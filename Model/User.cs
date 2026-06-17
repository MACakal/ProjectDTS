namespace ProjectDTS;

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

    // Role is stored and used as a plain string matching the name column in the roles table
    public string Role { get; set; } = "Customer";

    // Alias kept so existing display code compiles without changes
    public string RoleDisplayName => Role;

    public bool IsAdmin() => Role != "Customer";

    public User() { }
    public User(string name, string email, string password, string address = null, string zipCode = null, string country = null)
    {
        Name = name;
        Email = email;
        Password = password;
        Address = address;
        ZipCode = zipCode;
        Country = country;
        Role = "Customer";
    }
}
