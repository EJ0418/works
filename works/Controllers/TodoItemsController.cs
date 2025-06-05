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
            Summary = "取得所有待辦事項（分頁）",
            Description = "撈取資料庫中所有待辦事項，支援分頁查詢.")]
        [SwaggerResponse(200, "成功取得待辦事項列表", typeof(IEnumerable<TodoItem>))]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll(
            [FromQuery][SwaggerParameter("頁碼（從1開始）")] int page = 1,
            [FromQuery][SwaggerParameter("每頁筆數")] int pageSize = 15)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 15;

            var items = await _context.TodoItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return items;
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
                return NotFound("找不到指定id的待辦事項");
            }

            return todoItem;
        }

        [HttpPut()]
        [SwaggerOperation(
            Summary = "更新待辦事項",
            Description = "依據id更新待辦事項.")]
        [SwaggerResponse(200, "成功更新待辦事項", typeof(IEnumerable<TodoItem>))]
        [SwaggerResponse(404, "找不到指定id的待辦事項")]
        public async Task<IActionResult> UpdateTodoItem([SwaggerParameter("待辦事項內容")] TodoItem todoItem)
        {
            var varifyItem = await _context.TodoItems.FindAsync(todoItem.Id);
            if (varifyItem == null)
            {
                return NotFound("找不到指定id的待辦事項");
            }
            
            _context.Entry(varifyItem).CurrentValues.SetValues(todoItem);
            await _context.SaveChangesAsync();
             return Ok("成功更新待辦事項");
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "新增待辦事項",
            Description = "新增一個待辦事項.")]
        [SwaggerResponse(201, "成功新增待辦事項", typeof(IEnumerable<TodoItem>))]
        public async Task<ActionResult<TodoItem>> CreateTodoItem([SwaggerParameter("待辦事項內容")] TodoItem todoItem)
        {
            
            _context.TodoItems.Add(todoItem);
            int id = await _context.SaveChangesAsync();
            todoItem.Id = id;
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
                return NotFound("找不到指定id的待辦事項");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok("成功刪除待辦事項");
        }
    }
}
