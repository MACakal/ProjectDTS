// namespace ProjectDTS.Model
// {
namespace ProjectDTS;

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

    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    public UserRole Role { get; private set; }


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
// }