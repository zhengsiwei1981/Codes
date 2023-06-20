using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Map
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class ApiVersionController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("1.0");
        }
        [MapToApiVersion("2.0")]
        [HttpGet("{id}")]
        public ActionResult Index(int id)
        {
            return Ok("2.0");
        }
    }
}
