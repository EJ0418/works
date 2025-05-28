using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace SigninAPI.Controller
{
    public class SigninController : ControllerBase
    {
        [HttpPost("signin")]
        [SwaggerOperation(
            Summary = "登入",
            Description = "用於登入使用者。")]
        [SwaggerResponse(200, "登入成功")]
        public IActionResult Signin([SwaggerParameter("使用者名稱", Required = true)] string username, [SwaggerParameter("密碼", Required = true)] string password)
        {
            try
            {
                if (username == "test" && password == "0000")
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "User")
                };
                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync("CookieAuth", principal).Wait();
                    return Ok("登入成功");
                }
                return Unauthorized("使用者名稱或密碼錯誤");
            }
            catch (Exception ex)
            {
                return BadRequest($"登入失敗: {ex.Message}");
            }
        }

        [HttpPost("signout")]
        [SwaggerOperation(
            Summary = "登出",
            Description = "用於登出當前使用者。")]
        [SwaggerResponse(200, "成功登出")]
        public IActionResult Signout()
        {
            try
            {
                var user = HttpContext.User;
                if (user.Identity.IsAuthenticated)
                {
                    HttpContext.SignOutAsync("CookieAuth").Wait();
                    return Ok("成功登出");
                }
                return BadRequest("使用者未登入");
            }
            catch (Exception ex)
            {
                return BadRequest($"登出失敗: {ex.Message}");
            }

        }
    }
}