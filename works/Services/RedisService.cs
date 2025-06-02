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
        var pw = config["REDIS_PASSWORD"];
        var host = config["REDIS_HOST"];
        var port = config["REDIS_PORT"];


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
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(key), "請輸入key和value值");
        }
        await _db.StringSetAsync(key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), "請輸入key值");
        }
        var result = await _db.StringGetAsync(key);
        if (result.IsNullOrEmpty)
        {
            return null;
        }
        return result;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        if( string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), "請輸入key值");
        }
        if (!await _db.KeyExistsAsync(key))
        {
            throw new KeyNotFoundException($"Key:'{key}'不存在redis中");
        }
        return await _db.KeyDeleteAsync(key);
    }
}