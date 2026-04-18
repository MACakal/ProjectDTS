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

        // string sql = @"SELECT id, name, email, password, role
        //     FROM users
        //     WHERE email=@email AND pgp_sym_decrypt(password::bytea, 'admin_key')= @password";

        string sql = @"SELECT id, name, email, password, role
                        FROM users
                        WHERE email=@email";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", email);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        string dbPassword = reader.GetString(3);
        if (!BCrypt.Net.BCrypt.Verify(password, dbPassword)) return null;
        // cmd.Parameters.AddWithValue("password", password);



        return new User
        {

            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            // Password = reader.GetString(3),
            Role = Enum.Parse<UserRole>(reader.GetString(4), true)
        };

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

        sql = @"SELECT password FROM users WHERE email=@email";
        using var cmd1 = new NpgsqlCommand(sql, conn);
        cmd1.Parameters.AddWithValue("email", email);
        rowsAffected = Convert.ToInt32(cmd1.ExecuteScalar());

        if (rowsAffected > 0)
        {
            return UserRegisterService.alreadyExists;
        }
        string hash = BCrypt.Net.BCrypt.HashPassword(password);

        sql = @"INSERT INTO users (name, email, password) VALUES (@name, @email, @password)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password", hash);
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

    public bool EmailExists(string email, int currentUserId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT 1 FROM users WHERE email=@email AND id != @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("id", currentUserId);

        using var reader = cmd.ExecuteReader();

        return reader.Read();
    }

    public UserRegisterService UserInformationPasswordCheck(User user, string password)
    {
        if (user == null || string.IsNullOrWhiteSpace(password))
        {
            return UserRegisterService.emptyParameter;
        }
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"SELECT password FROM users WHERE email=@email";

        using var cmd = new NpgsqlCommand(sql, conn);

        // var dbPassword = (string?)cmd.ExecuteScalar();

        cmd.Parameters.AddWithValue("email", user.Email);

        var dbPassword = (string?)cmd.ExecuteScalar();

        if (dbPassword == null)

            return UserRegisterService.UnkownError;

        if (BCrypt.Net.BCrypt.Verify(password, dbPassword))

            return UserRegisterService.succesfull;

        return UserRegisterService.UnkownError;
    }
    public List<UserSpendingView> GetUserSpending()
    {
        var result = new List<UserSpendingView>();
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"
        SELECT id, name, COALESCE (SUM(total_price), 0) AS total_spending
        FROM user_spending
        GROUP BY id, name
        ORDER BY total_spending DESC";

        // WHERE order_date IS NOT NULL AND DATE_TRUNC('month', order_date) = DATE_TRUNC('month', CURRENT_DATE)
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



    // public UserRegisterService UpdateUser(User user)
    // {
    //     if (user == null)
    //     {
    //         return UserRegisterService.emptyParameter;
    //     }

    //     using var conn = _db.GetConnection();
    //     conn.Open();
    //     string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);
    //     string sql = @"UPDATE users 
    //                 SET name=@name, email=@email, password=@password
    //                 WHERE id=@id";

    //     using var cmd = new NpgsqlCommand(sql, conn);
    //     cmd.Parameters.AddWithValue("name", user.Name);
    //     cmd.Parameters.AddWithValue("email", user.Email);
    //     cmd.Parameters.AddWithValue("password", hash);
    //     cmd.Parameters.AddWithValue("id", user.Id);

    //     int rows = cmd.ExecuteNonQuery();

    //     if (rows > 0)
    //     {
    //         return UserRegisterService.succesfull;
    //     }

    //     return UserRegisterService.UnkownError;
    // }

    public UserRegisterService UpdateUser(User user)
    {
        if (user == null)
            return UserRegisterService.emptyParameter;

        using var conn = _db.GetConnection();
        conn.Open();

        string sql;
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;


        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            sql = @"UPDATE users 
                SET name=@name, email=@email, password=@password
                WHERE id=@id";

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("name", user.Name);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("password", hash);
            cmd.Parameters.AddWithValue("id", user.Id);
        }
        else
        {
            sql = @"UPDATE users 
                SET name=@name, email=@email
                WHERE id=@id";

            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("name", user.Name);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("id", user.Id);
        }

        int rows = cmd.ExecuteNonQuery();

        return rows > 0
            ? UserRegisterService.succesfull
            : UserRegisterService.UnkownError;
    }
    public UserRegisterService DeleteUser(int userId)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string sql = @"DELETE FROM users WHERE id=@id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", userId);

        int rows = cmd.ExecuteNonQuery();

        if (rows > 0)
        {
            return UserRegisterService.succesfull;
        }

        return UserRegisterService.UnkownError;
    }

    // public void UpdateProfile(string name, string address, string zipCode, string country)
    // {
    //     using var conn = _db.GetConnection();
    //     conn.Open();

    //     string sql = @"
    //     UPDATE users 
    //     SET 
    //         name = @Name,
    //         address =@Address,
    //         Zip_code = @ZipCode,
    //         country = @Country
    //         WHERE Role = Admin";
    // }
}


public enum UserRegisterService
{
    succesfull,
    emptyParameter,
    alreadyExists,
    UnkownError
}
