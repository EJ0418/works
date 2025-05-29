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
                { "Redis:Password", "test0000" },
                { "Redis:Host", "localhost" },
                { "Redis:Port", "6379" }
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
            var result =  
        }

        [Test]
        public async Task Get_ReturnsOk_WhenValueExists()
        {
            var key = "1";
            var value = "Test";
            _redisMock.Setup(r => r.GetAsync(key)).ReturnsAsync(value);

            var result = await _controller.Get(key);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(value, okResult.Value);
        }

        [Test]
        public async Task Get_ReturnsNotFound_WhenValueDoesNotExist()
        {
            var key = "1";
            _redisMock.Setup(r => r.GetAsync(key)).ReturnsAsync((string)null);

            var result = await _controller.Get(key);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            var key = "1";
            _redisMock.Setup(r => r.DeleteAsync(key)).ReturnsAsync(true);

            var result = await _controller.Delete(key);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenNotDeleted()
        {
            var key = "1";
            _redisMock.Setup(r => r.DeleteAsync(key)).ReturnsAsync(false);

            var result = await _controller.Delete(key);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}