using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapi.MyExtension;

namespace Webapi.Controllers.DataProtection
{

    [Route("api/[controller]")]
    [ApiController]
    public class DataProtectionController : ControllerBase
    {
        [HttpGet("/encypt/{val}")]
        public IActionResult Encypt(string val, [FromServices] IDataProtectionProvider dataProtectionProvider)
        {
            var v = dataProtectionProvider.CreateProtector("test").Protect(val);
            return Ok(v);
        }
        [HttpGet("/decypt/{val}")]
        public IActionResult Decypt(string val, [FromServices] IDataProtectionProvider dataProtectionProvider)
        {
            var v = dataProtectionProvider.CreateProtector("test").Unprotect(val);
            return Ok(v);
        }
        [HttpPost("/encypt/{val}/{second}")]
        public IActionResult Encypt(string val, int second, [FromServices] IDataProtectionProvider dataProtectionProvider)
        {
            var v = dataProtectionProvider.CreateProtector("test").ToTimeLimitedDataProtector().CreateProtector("test").Protect(val, TimeSpan.FromSeconds(second));
            return Ok(v);
        }
        [HttpGet("/decypt_timeout/{val}")]
        public IActionResult Decypt2(string val, [FromServices] IDataProtectionProvider dataProtectionProvider)
        {
            var v = dataProtectionProvider.CreateProtector("test").ToTimeLimitedDataProtector().CreateProtector("test").Unprotect(val);
            return Ok(v);
        }
        [HttpGet("/revoke")]
        public IActionResult RevokeKey([FromServices] IKeyRingProvider keyRingProvider, [FromServices] IKeyManager keyManager)
        {
            var keyRing = keyRingProvider.GetCurrentKeyRing();
            var keyId = keyRing.DefaultKeyId;
            keyManager.RevokeKey(keyId);

            return Ok();
        }
        [HttpGet("/elements")]
        [Produces("application/json")]
        public IActionResult Elements([FromServices] DataProtectionDbContext dataProtectionDbContext)
        {
            return Ok(dataProtectionDbContext.DataProtectionKeys.ToList());
        }
        /// <summary>
        /// 此方法需要右键项目图标单击使用管理用户密钥功能，在打开的secret.json文件当中添加需要保护的敏感资源，之后通过配置的方式进行访问
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpGet("/key")]
        [Produces("application/json")]
        public IActionResult Key([FromServices] IConfiguration configuration)
        {
            return Ok(configuration["Movies:ServiceApiKey"]);
        }
    }
}
