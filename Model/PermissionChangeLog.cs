using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class PermissionChangeLog
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int? UserId { get; set; }
    public string UserSessionId { get; set; } = "";
    public string ActionType { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "";
    public List<string> OldPermissions { get; set; } = new();
    public List<string> NewPermissions { get; set; } = new();
}
