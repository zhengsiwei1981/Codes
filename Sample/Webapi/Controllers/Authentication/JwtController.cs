using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Webapi.MyExtension;

namespace Webapi.Controllers.Authentication
{
    /// <summary>
    /// 使用postman 访问
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        [HttpGet("Auth")]
        [Produces("application/json")]
        public async Task<IActionResult> Get()
        {
            var result = await this.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                var user = result.Principal.Identities.FirstOrDefault();
                return Ok(user);
            }
            else if (result.Failure != null)
            {
                return Problem(result.Failure.Message);
            }
            else
            {
                await this.HttpContext.ChallengeAsync(JwtBearerDefaults.AuthenticationScheme);
            }
            return Problem("no auth");
        }
        [HttpGet("token/{role}/{name}")]
        public IActionResult GetToken(string role, string name, [FromServices] IOptions<Token> option)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, name)
            };

            var cred = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.Value.SecretKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(option.Value.Issuer, option.Value.Audience, claims, expires: DateTime.Now.AddHours(2), signingCredentials: cred);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenString);
        }
        /// <summary>
        /// 验证权限，在postman调用
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Verify")]
        public IActionResult VerifyAdminRole()
        {
            return Ok("Successed");
        }
    }
}
