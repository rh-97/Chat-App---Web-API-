using Newtonsoft.Json;
using StackExchange.Redis;

namespace Test.Services;

public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }
    public async Task<List<string>> getAll()
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        var endPoint = _connectionMultiplexer.GetEndPoints().First();
        var keys = _connectionMultiplexer.GetServer(endPoint).KeysAsync(db.Database, "dr-*");
        List<string> keyList = new List<string>();
        await foreach (var key in keys)
        {
            keyList.Add(key.ToString());
        }
        return keyList;
    }

    public async Task Set<T>(string key, T value)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromMinutes(1));
    }

    public async Task Delete(string key)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
