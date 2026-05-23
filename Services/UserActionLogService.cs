using MongoDB.Driver;

public class UserActionLogService
{
    private readonly IMongoCollection<UserActionLog> _userActionLogs;

    public UserActionLogService(MongoDbContext context)
    {
        _userActionLogs = context.GetCollection<UserActionLog>("user_action_logs");
    }

    public async Task SaveUserActionLogAsync(UserActionLog log)
    {
        await _userActionLogs.InsertOneAsync(log);
    }
}