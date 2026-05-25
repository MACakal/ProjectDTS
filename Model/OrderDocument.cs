using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class OrderDocument
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int UserId { get; set; }
    public int PostgresOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderStatusEntry> StatusHistory { get; set; } = new();
    public List<BasketItem> Products { get; set; }
}
