namespace ProjectDTS;
using MongoDB.Driver;
public class ProductAuditLogService

{
    private readonly IMongoCollection<ProductLog> _logs;

    public ProductAuditLogService(MongoDbContext context)
    {
        _logs = context.GetCollection<ProductLog>("product_audit_logs");
    }

    public async Task LogAsync(string action, int userId, Product product)
    {

        var log = new ProductLog
        {
            ProductId = product.Id,
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            ProductName = product.Name,
            ProductPrice = product.Price,
            ProductStock = product.Stock
        };

        await _logs.InsertOneAsync(log);
    }

    public async Task<List<ProductLog>> GetLogsForProductAsync(int productId) // ← ProductLog
    {
        return await _logs
            .Find(log => log.ProductId == productId)
            .SortByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<List<ProductLog>> GetAllLogsAsync()
    {
        return await _logs
            .Find(_ => true)
            .SortByDescending(log => log.Timestamp)
            .ToListAsync();
    }
}