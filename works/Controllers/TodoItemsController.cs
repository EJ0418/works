using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Models;

namespace works.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "取得所有待辦事項",
            Description = "撈取memory中所有待辦事項.")]
        [SwaggerResponse(200, "成功取得待辦事項列表", typeof(IEnumerable<TodoItem>))]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "取得待辦事項（單一）",
            Description = "依據ID撈取單一待辦事項.")]

        [SwaggerResponse(200, "成功取得待辦事項", typeof(IEnumerable<TodoItem>))]
        [SwaggerResponse(404, "找不到指定id的待辦事項")]
        public async Task<ActionResult<TodoItem>> GetTodoItem([SwaggerParameter("待辦事項ID")] int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        [HttpPut()]
        [SwaggerOperation(
            Summary = "更新待辦事項",
            Description = "依據id更新待辦事項.")]
        [SwaggerResponse(200, "成功更新待辦事項", typeof(IEnumerable<TodoItem>))]
        [SwaggerResponse(400, "欄位的id與待輸入的json id不符")]
        [SwaggerResponse(404, "找不到指定id的待辦事項")]
        public async Task<IActionResult> UpdateTodoItem([SwaggerParameter("待辦事項內容")] TodoItem todoItem)
        {
            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "新增待辦事項",
            Description = "新增一個待辦事項.")]
        [SwaggerResponse(200, "成功新增待辦事項", typeof(IEnumerable<TodoItem>))]
        public async Task<ActionResult<TodoItem>> CreateTodoItem([SwaggerParameter("待辦事項內容")] TodoItem todoItem)
        {
            
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }


        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "刪除待辦事項",
            Description = "依據id刪除待辦事項.")]
        [SwaggerResponse(200, "成功刪除待辦事項", typeof(IEnumerable<TodoItem>))]
        [SwaggerResponse(404, "找不到指定id的待辦事項")]
        public async Task<IActionResult> DeleteTodoItem([SwaggerParameter("待辦事項ID")]int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
