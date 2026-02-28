public enum UserRole
{
    Customer,
    Admin
}
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string Address { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }

    public UserRole Role { get; set; }
}