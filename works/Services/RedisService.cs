using System.Configuration;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;

public class RedisService
{
    private readonly IDatabase _db;

    public RedisService(IConfiguration config)
    {
        var user = config["REDIS_USER"];
        var pw = config["REDIS_PASSWORD"];
        var host = config["REDIS_HOST"];
        var port = config["REDIS_PORT"];


        var opt = new ConfigurationOptions
        {
            EndPoints = { $"{host}:{port}" },
            User = user,
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
        var result = await _db.StringGetAsync(key);
        if (result.IsNullOrEmpty)
        {
            return null;
        }
        return result;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        if (!await _db.KeyExistsAsync(key))
        {
            throw new KeyNotFoundException($"Key:'{key}'不存在redis中");
        }
        return await _db.KeyDeleteAsync(key);
    }
}