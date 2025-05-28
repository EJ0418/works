using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TodoApi.Models;
using works.Controllers;

namespace works.Controllers
{
    [TestFixture]
    public class CRUD_Test
    {
        private List<TodoItem> GetTestTodoItems()
        {
            return new List<TodoItem>
            {
                new TodoItem { Id = 1, Name = "TodoItem 1", IsComplete = false },
                new TodoItem { Id = 2, Name = "TodoItem 2", IsComplete = true }
            };
        }

        private TodoContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDb")
                .Options;
            var context = new TodoContext(options);
            if (!context.TodoItems.Any())
            {
                context.TodoItems.AddRange(GetTestTodoItems());
                context.SaveChanges();
            }
            return context;
        }

        [Test, Order(1)]
        public async Task GetTodoItems_ReturnsAllItems()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var result = await controller.GetTodoItems();
            var items = result.Value as IEnumerable<TodoItem>;
            Assert.IsNotNull(items);
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test, Order(2)]
        public async Task GetTodoItem_ReturnsItem_WhenExists()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var result = await controller.GetTodoItem(1);
            var item = result.Value as TodoItem;
            Assert.IsNotNull(item);
            Assert.AreEqual(1, item.Id);
        }

        [Test, Order(3)]
        public async Task GetTodoItem_ReturnsNotFound_WhenNotExists()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var result = await controller.GetTodoItem(999);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test, Order(5)]
        public async Task PutTodoItem_ReturnsBadRequest_WhenIdMismatch()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var todoItem = new TodoItem { Id = 2, Name = "Test", IsComplete = false };
            var result = await controller.PutTodoItem(1, todoItem);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test, Order(6)]
        public async Task PutTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var todoItem = new TodoItem { Id = 999, Name = "Test", IsComplete = false };
            var result = await controller.PutTodoItem(999, todoItem);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test, Order(7)]
        public async Task PutTodoItem_UpdatesItem_WhenValid()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var todoItem = new TodoItem { Id = 1, Name = "Updated", IsComplete = true };
            var result = await controller.PutTodoItem(1, todoItem);
            Assert.IsInstanceOf<NoContentResult>(result);
            var updated = await context.TodoItems.FindAsync(1L);
            Assert.AreEqual("Updated", updated.Name);
            Assert.IsTrue(updated.IsComplete);
        }

        [Test, Order(4)]
        public async Task PostTodoItem_CreatesItem()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var todoItem = new TodoItem { Name = "New Item", IsComplete = false };
            var result = await controller.PostTodoItem(todoItem);
            var createdAt = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAt);
            var created = createdAt.Value as TodoItem;
            Assert.IsNotNull(created);
            Assert.AreEqual("New Item", created.Name);
        }

        [Test, Order(8)]
        public async Task DeleteTodoItem_RemovesItem_WhenExists()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var result = await controller.DeleteTodoItem(1);
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.IsNull(await context.TodoItems.FindAsync(1L));
        }

        [Test, Order(9)]
        public async Task DeleteTodoItem_ReturnsNotFound_WhenNotExists()
        {
            var context = GetDbContextWithData();
            var controller = new TodoItemsController(context);
            var result = await controller.DeleteTodoItem(999);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}