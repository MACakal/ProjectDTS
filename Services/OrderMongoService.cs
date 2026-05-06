using MongoDB.Driver;

public class OrderMongoService
{
    private readonly IMongoCollection<OrderDocument> _orders;

    public OrderMongoService(MongoDbContext context)
    {
        _orders = context.GetCollection<OrderDocument>("orders");
    }

    public async Task SaveOrderAsync(OrderDocument order)
    {
        await _orders.InsertOneAsync(order);
    }
}