using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Webapi.Controllers.Authorization
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet("adduser")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUser(string name)
        {
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role,"admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(principal);
            return Ok(new { Name = name, Role = "admin" });
        }
        [HttpGet("role")]
        [Authorize(Policy = "RequireAdmin")]
        public IActionResult VisitByRole()
        {
            return Ok("Successed");
        }
        [HttpGet("name")]
        [Authorize(Policy = "RequireName")]
        public IActionResult VisitByName()
        {
            return Ok();
        }
        [HttpGet("claim")]
        [Authorize(Policy = "ClaimCount")]
        public IActionResult Count()
        {
            return Ok("Successed");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("AccessDenied")]
        public IActionResult Denied()
        {
            var result = Problem("no access auth");
            return new JsonResult(result);
        }
    }
}
