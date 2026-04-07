using ProjectDTS;
using Npgsql;
public class NotificationService
{
    private readonly DatabaseService _db;

    public NotificationService(DatabaseService db)
    {
        _db = db;
    }

    public List<Notification> GetNotifications()
    {
        var notifications = new List<Notification>();
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = @"SELECT id, message, created_at, is_read FROM admin_notifications ORDER BY created_at DESC LIMIT 20";
        
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            notifications.Add(new Notification(
                reader.GetInt32(0),
                (0),
                reader.GetString(1),
                reader.GetDateTime(2),
                reader.GetBoolean(3)
            ));
        }
        return notifications;
    }

    public void MarkAllAsRead()
    {
        using var conn = _db.GetConnection();
        conn.Open();
        string sql = "UPDATE admin_notifications SET is_read = true WHERE is_read = false;";
        
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
}