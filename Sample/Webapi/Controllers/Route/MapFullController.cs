using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Route
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MapFullController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "Index";
        }
    }
}
