using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerFormatController : ControllerBase
    {
        [HttpGet]
        [Produces("text/mytest")]
        public MyTest GetMyTest()
        {
            return new MyTest() { Name = "111", Value = "222" };
        }
        [HttpGet]
        [Produces("text/mytest")]
        public IEnumerable<MyTest> GetMyTestList()
        {
            return new List<MyTest>() { new MyTest() { Name = "111", Value = "222" }, new MyTest() { Name = "333", Value = "444" } };
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SetMyTest(MyTest myTest)
        {
            return Ok();
        }
    }
}
