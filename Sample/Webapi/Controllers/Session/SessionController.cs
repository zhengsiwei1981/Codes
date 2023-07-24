using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Session
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        [HttpGet("set")]
        public async Task<IActionResult> Set(string key, string val)
        {
            await this.HttpContext.Session.LoadAsync();
            this.HttpContext.Session.SetString(key, val);
            return this.Ok();
        }
        [HttpGet("get")]
        public async Task<IActionResult> Get(string key)
        {
            await this.HttpContext.Session.LoadAsync();
            return this.Ok(this.HttpContext.Session.GetString(key));
        }
    }
}
