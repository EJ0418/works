using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using works.Controllers;
using TodoApi.Models;
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
            var inMemrySettings = new Dictionary<string, string>
            {
                { "REDIS_PASSWORD", "redis_pw" },
                { "REDIS_HOST", "localhost" },
                { "REDIS_PORT", "6379" }
            };

            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemrySettings)
                .Build();

            _redisSerivice = new RedisService(config);
        }

        [Test, Order(1)]
        public async Task Set_ReturnsOk()
        {
            var item = new TodoItem { Id = 1, Name = "Test" };
            await _redisSerivice.SetAsync(item.Id.ToString(), item.Name);
            var result = _redisSerivice.GetAsync(item.Id.ToString());
            Assert.IsNotNull(result);
            Assert.AreEqual(item.Name, result.Result);
        }

        [Test, Order(2)]
        public async Task Set_UpdateItem()
        {
            var item = new TodoItem { Id = 1, Name = "update" };
            await _redisSerivice.SetAsync(item.Id.ToString(), item.Name);
            var result = _redisSerivice.GetAsync(item.Id.ToString());
            Assert.IsNotNull(result);
            Assert.AreEqual(item.Name, result.Result);
        }

        [Test, Order(3)]
        public async Task Get_ReturnsOk_WhenValueExists()
        {
            var result = _redisSerivice.GetAsync("1");
            Assert.IsNotNull(result);
        }

        [Test, Order(4)]
        public async Task Get_ReturnsNotFound_WhenValueDoesNotExist()
        {
            var result = _redisSerivice.GetAsync("999");
            Assert.IsNull(result.Result);
        }

        [Test, Order(5)]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var key = "1";
            await _redisSerivice.DeleteAsync(key);
            var result = await _redisSerivice.GetAsync(key);
            Assert.IsNull(result);
        }

        [Test, Order(6)]
        public async Task Delete_ReturnsNotFound_WhenNotDeleted()
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