using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class UserActionLog
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int? UserId { get; set; }
    public string UserSessionId { get; set; } = "";
    public string ActionType { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Details { get; set; } = new();
}