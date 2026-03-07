// using Npgsql;
// namespace ProjectDTS;

// public class AdminService
// {
//     private readonly DatabaseService _db;

//     public AdminService(DatabaseService db)
//     {
//         _db = db;
//     }
//     public List<User> GetAdmins()
//     {

//         List<User> admins = new List<User>();
//         using var conn = _db.GetConnection();
//         conn.Open();

//         string sql = @"SELECT name, email, password, role
//          FROM users 
//          WHERE role = 'Admin'";

//         using var cmd = new NpgsqlCommand(sql, conn);
//         using var reader = cmd.ExecuteReader();

//         while (reader.Read())
//         {
//             admins.Add(new User
//             {
//                 Name = reader.GetString(0),
//                 Email = reader.GetString(1),
//                 Password = reader.GetString(2),
//                 Role = Enum.Parse<UserRole>(reader.GetString(3))
//             });

//         }
//         return admins;

//     }



// }

