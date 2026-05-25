namespace ProjectDTS;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ProductLog
{
    [BsonId]
    public ObjectId Id { get; set; }

    public int ProductId { get; set; }
    public string Action { get; set; }
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string ProductName { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductStock { get; set; }
}