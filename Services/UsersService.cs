using Npgsql;
using NpgsqlTypes;
namespace ProjectDTS;

public class UserService
{
    private DatabaseService _db;
    public UserService(DatabaseService db)
    {
        _db = db;
    }
    public User? UserLogin(string email, string password)
    {
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = @"SELECT id, name, email, password, role
            FROM users
            WHERE email=@email AND password = @password";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password", password);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new User
            {

                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3),
                Role = Enum.Parse<UserRole>(reader.GetString(4))
            };
        }
        return null;
    }
    public UserRegisterService UserRegister(string email, string password)
    {
        if (email == null|| password == null)
        {
            return UserRegisterService.emptyParameter;
        }
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = @"INSERT INTO users WHERE email=@email AND password=@password";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password", password);
        var rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            return UserRegisterService.succesfull;
        }
        else
        {
            return UserRegisterService.UnkownError;
        }
    }

}

public enum UserRegisterService
{
    succesfull,
    emptyParameter,
    UnkownError
}