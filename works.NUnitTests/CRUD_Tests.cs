using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using NuGet.ContentModel;
using NUnit.Framework;
using TodoApi.Models;
using works.Controllers;

namespace works.Controllers
{
    [TestFixture]
    public class CRUD_Test
    {
        private TodoContext _context;
        private int _testId = 19;

        [SetUp]
        public void SetUp()
        {
            // DotNetEnv.Env.Load("/Users/ej/work/works/works.NUnitTests/.env");
            // var server = DotNetEnv.Env.GetString("DB_SERVER");
            // var db = Environment.GetEnvironmentVariable("DB_DB");
            // var user = Environment.GetEnvironmentVariable("DB_USER");
            // var pw = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseMySql(
                    $"server=127.0.0.1;database=tododb;user=ej;password=ej_pw;",
                    new MySqlServerVersion(new Version(10, 5, 0)))
                .Options;
            _context = new TodoContext(options);
        }

        [Test, Order(1)]
        public async Task CreateTodoItem()
        {
            var controller = new TodoItemsController(_context);
            await controller.CreateTodoItem(new TodoItem() { Title = "testCase", IsDone = false });

            var getResult = await controller.GetTodoItem(_testId);
            Assert.AreEqual(getResult.Value.Title, "testCase");
        }

        [Test, Order(2)]
        public async Task Update_WhenIdExists()
        {
            var controller = new TodoItemsController(_context);
            await controller.UpdateTodoItem(new TodoItem() { Id = 1, Title = "testCase_update_" + _testId.ToString(), IsDone = true });

            var getResult = await controller.GetTodoItem(1);

            Assert.AreEqual(getResult.Value.Title, "testCase_update_" + _testId.ToString());
        }

        [Test, Order(3)]
        public async Task Update_WhenIdNotExists()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.UpdateTodoItem(new TodoItem() { Id = 999, Title = "testCase_update", IsDone = true });

            Assert.IsInstanceOf<NotFoundResult>(result);

        }

        [Test, Order(4)]
        public async Task GetAllItems()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.GetAll();
            var items = result.Value;
            Assert.IsNotNull(items);
            Assert.That(items.Count(), Is.AtLeast(1));
        }

        [Test, Order(5)]
        public async Task GetTodoItem_WhenExists()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.GetTodoItem(_testId);
            var item = result.Value;
            Assert.IsNotNull(item);
            Assert.AreEqual(_testId, item.Id);
        }

        [Test, Order(6)]
        public async Task GetTodoItem_WhenNotExists()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.GetTodoItem(999);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }



        [Test, Order(7)]
        public async Task DeleteTodoItem_WhenExists()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.DeleteTodoItem(_testId);
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.IsNull(await _context.TodoItems.FindAsync(_testId));
        }

        [Test, Order(8)]
        public async Task DeleteTodoItem_WhenNotExists()
        {
            var controller = new TodoItemsController(_context);
            var result = await controller.DeleteTodoItem(999);
            Assert.IsInstanceOf<NotFoundResult>(result);

        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}