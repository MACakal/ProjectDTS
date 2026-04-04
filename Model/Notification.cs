public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsRead { get; set; }

    public Notification(int id, int userId, string message, DateTime createdAt, bool isRead = false)
    {
        Id = id;
        UserId = userId;
        Message = message;
        CreatedAt = createdAt;
        IsRead = isRead;
    }
}