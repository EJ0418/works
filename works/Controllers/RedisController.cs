using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Models;

namespace works.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class RedisController : ControllerBase
    {
        private readonly RedisService _redis;

        public RedisController(RedisService redis)
        {
            _redis = redis;
        }

        [HttpPost("set")]
        [SwaggerOperation(
            Summary = "新增",
            Description = "新增一個項目到 Redis 中。200 OK 表示成功，並且不return內容。")]
        [SwaggerResponse(200, "新增成功", typeof(void))]
        public async Task<IActionResult> Set([SwaggerParameter("新增ID", Required =true)] string id, [SwaggerParameter("新增名稱", Required =true)] string name)
        {
            await _redis.SetAsync(id, name);
            return Ok();
        }

        [HttpGet("get")]
        [SwaggerOperation(
            Summary = "查詢",
            Description = "查詢指定的 key 在 Redis 中的值。")]
        [SwaggerResponse(200, "查詢成功", typeof(string))]
        [SwaggerResponse(404, "查詢失敗", typeof(void))]
        public async Task<IActionResult> Get([SwaggerParameter("查詢ID", Required =true)] string id)
        {
            var value = await _redis.GetAsync(id);
            if (value == null)
            {
                return NotFound();
            }
            return Ok(value);
        }


        [HttpDelete("delete")]
        [SwaggerOperation(
            Summary = "刪除",
            Description = "刪除指定的 key 在 Redis 中的值。")]
        [SwaggerResponse(204, "刪除成功", typeof(void))]
        [SwaggerResponse(400, "尚未輸入ID", typeof(void))]
        [SwaggerResponse(404, "刪除失敗", typeof(void))]
        public async Task<IActionResult> Delete([SwaggerParameter("刪除ID", Required =true)] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("請輸入ID");
            }
            var deleted = await _redis.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
