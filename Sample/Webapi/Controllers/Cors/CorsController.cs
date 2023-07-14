using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Cors
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorsController : ControllerBase
    {
        [HttpGet("corstest")]
        [EnableCors("mypolicy")]
        public string Get()
        {
            return "success";
        }
    }
}
