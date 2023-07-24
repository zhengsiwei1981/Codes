using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Webapi.MyExtension;

namespace Webapi.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class CookiesController : ControllerBase
    {
        [HttpGet("SignIn")]
        public async Task<IActionResult> Login(string name, string password, [FromServices] ILogger<CookiesController> logger)
        {
            //模拟登录
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role,"admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(principal);
            logger.LogInformation($" {name} login , role : admin ");
            return Ok(principal);
        }
        //模拟等处
        [HttpGet("SignOut")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync();
            return Ok("Successed");
        }
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("Verify")]
        public IActionResult VerifyAdminRole()
        {
            return Ok("Successed");
        }
        /// <summary>
        /// 拒绝访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("DenyAccess")]
        public IActionResult DenyAccess()
        {
            return Problem("no auth or no login");
        }
    }
}
