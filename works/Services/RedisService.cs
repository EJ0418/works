using System.Configuration;
using StackExchange.Redis;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(IConfiguration config)
    {
        var pw = config["Redis:Password"];
        var host = config["Redis:Host"];
        var port = config["Redis:Port"];

        var opt = new ConfigurationOptions
        {
            EndPoints = { $"{host}:{port}" },
            Password = pw,
            AbortOnConnectFail = false
        };
        var connection = ConnectionMultiplexer.Connect(opt);
        _db = connection.GetDatabase();
    }

    public async Task SetAsync(string key, string value)
    {
        await _db.StringSetAsync(key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }
}