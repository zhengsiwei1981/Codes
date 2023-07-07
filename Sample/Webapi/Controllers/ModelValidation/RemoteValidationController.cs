using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.Results;

namespace Webapi.Controllers.ModelValidation
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RemoteValidationController : ControllerBase
    {
        [HttpGet]
        public IActionResult VerifyName(string firstName, string lastName)
        {
            if (firstName == null || lastName == null)
            {
                return new JsonResult(false);
            }
            return new JsonResult(true);
        }
    }
}
