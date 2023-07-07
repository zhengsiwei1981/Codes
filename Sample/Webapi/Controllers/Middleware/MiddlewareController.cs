using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Middleware
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiddlewareController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok();
        }
    }
}
