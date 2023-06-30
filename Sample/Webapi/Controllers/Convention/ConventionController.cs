using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Webapi.Controllers.Convention
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConventionController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [ApiConventionMethod(typeof(MyAppConventions),
                     nameof(MyAppConventions.Find))]
        public IActionResult Find(int id)
        {
            return Ok();
        }
    }
    public static class MyAppConventions
    {
        // Adjusted Version
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Find(
            [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] int id
        )
        {
            // A convention method that matches "Find[...]" controller methods
        }
    }
    public class Test
    {
        public string Name { get; set; }
    }
}
