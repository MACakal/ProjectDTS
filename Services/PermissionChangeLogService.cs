using MongoDB.Driver;

public class PermissionChangeLogService
{
    private readonly IMongoCollection<PermissionChangeLog> _permissionChangeLogs;

    public PermissionChangeLogService(MongoDbContext context)
    {
        _permissionChangeLogs = context.GetCollection<PermissionChangeLog>("permission_change_logs");
    }

    public async Task SavePermissionChangeLogAsync(PermissionChangeLog log)
    {
        await _permissionChangeLogs.InsertOneAsync(log);
    }
}
