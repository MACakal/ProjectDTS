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

    public async Task<List<OrderDocument>> GetOrdersByUserIdAsync(int userId)
    {
        return await _orders
            .Find(o => o.UserId == userId)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task AddStatusUpdateAsync(int postgresOrderId, string statusName)
    {
        var filter = Builders<OrderDocument>.Filter.Eq(o => o.PostgresOrderId, postgresOrderId);
        var update = Builders<OrderDocument>.Update.Push(o => o.StatusHistory, new OrderStatusEntry
        {
            StatusName = statusName,
            Timestamp = DateTime.UtcNow
        });
        await _orders.UpdateOneAsync(filter, update);
    }

    public async Task<List<OrderDocument>> GetAllOrdersAsync()
    {
        return await _orders
            .Find(_ => true)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync();
    }
}
