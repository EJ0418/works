using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using works.Controllers;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;

namespace works.Controllers
{
    public class RedisControllerTest
    {
        private RedisService _redisSerivice;

        [SetUp]
        public void Setup()
        {
            var inMemroySettings = new Dictionary<string, string>
            {
                { "REDIS_USER", "default" },
                { "REDIS_PASSWORD", "redis_pw" },
                { "REDIS_HOST", "localhost" },
                { "REDIS_PORT", "6379" }
            };

            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemroySettings)
                .Build();

            _redisSerivice = new RedisService(config);
        }

        [Test, Order(1)]
        public async Task Set_CreateItem()
        {
            await _redisSerivice.SetAsync("1", "Test");
            var result = _redisSerivice.GetAsync("1");
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Result);
        }

        [Test, Order(2)]
        public async Task Get_WhenValueExists()
        {
            var result = _redisSerivice.GetAsync("1");
            Assert.IsNotNull(result);
        }

        [Test, Order(3)]
        public async Task Get_WhenValueDoesNotExist()
        {
            var result = _redisSerivice.GetAsync("999");
            Assert.IsNull(result.Result);
        }

        [Test, Order(4)]
        public async Task Delete_WhenValueExists()
        {
            var key = "1";
            await _redisSerivice.DeleteAsync(key);
            var result = await _redisSerivice.GetAsync(key);
            Assert.IsNull(result);
        }

        [Test, Order(5)]
        public async Task Delete_WhenValueDoesNotExist()
        {
            try
            {
                var key = "999";
                await _redisSerivice.DeleteAsync(key);
            }
            catch (KeyNotFoundException ex)
            {
                Assert.IsInstanceOf<KeyNotFoundException>(ex);
                Assert.AreEqual("Key:'999'不存在redis中", ex.Message);
            }
        }
    }
}