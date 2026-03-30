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
            WHERE email=@email AND pgp_sym_decrypt(password::bytea, 'admin_key')= @password";

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
                Role = Enum.Parse<UserRole>(reader.GetString(4), true)
            };
        }
        return null;
    }
    public UserRegisterService UserRegister(string name, string email, string password)
    {
        string sql;
        int rowsAffected;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return UserRegisterService.emptyParameter;
        }
        using var conn = _db.GetConnection();
        conn.Open();

        sql = @"SELECT COUNT(*) FROM users WHERE email=@email";
        using var cmd1 = new NpgsqlCommand(sql, conn);
        cmd1.Parameters.AddWithValue("email", email);
        rowsAffected = Convert.ToInt32(cmd1.ExecuteScalar());

        if (rowsAffected > 0)
        {
            return UserRegisterService.alreadyExists;
        }

        sql = @"INSERT INTO users (name, email, password) VALUES (@name, @email, pgp_sym_encrypt(@password, 'admin_key'))";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password", password);
        rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            return UserRegisterService.succesfull;
        }
        else
        {
            return UserRegisterService.UnkownError;
        }
    }

    public UserRegisterService UserInformationPasswordCheck(User user, string password)
    {
        if (user == null || string.IsNullOrWhiteSpace(password))
        {
            return UserRegisterService.emptyParameter;
        }
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = @"SELECT * FROM users WHERE email=@email AND pgp_sym_decrypt(password::bytea, 'admin_key') = @password";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", user.Email);
        cmd.Parameters.AddWithValue("password", password);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return UserRegisterService.succesfull;
        }
        else
        {
            return UserRegisterService.UnkownError;
        }
    }
    public List<UserSpendingView> GetUserSpending()
    {
        var result = new List<UserSpendingView>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT * FROM user_spending";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new UserSpendingView
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                TotalSpending = reader.GetDecimal(2)
            }
            );
        }
        return result;
    }

}


public enum UserRegisterService
{
    succesfull,
    emptyParameter,
    alreadyExists,
    UnkownError
}
