using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    public class Test : ControllerBase
    {
        [HttpGet]
        [Route("crash")]
        public IActionResult Crash()
        {
            throw new Exception("This is a crash test.");
        }

        [Route("public")]
        [HttpGet]
        public IActionResult Public()
        {
            return Ok("This is public resource.");
        }

        [Authorize]
        [HttpGet]
        [Route("private")]
        public IActionResult Private()
        {
            return Ok("This is private resource.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromForm] String account, [FromForm] String password)
        {

            // 模擬驗證邏輯
            if (account == "test" && password == "0000")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account),
                };
                var identity = new ClaimsIdentity(claims, "cookie");
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync("CookieAuth", principal).Wait();
                return Ok("登入成功！");
            }
            return Unauthorized("Invalid credentials");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // 在這裡實現登出邏輯，例如清除 Cookie 或 JWT
            return Ok("已登出");
        }
    }
}