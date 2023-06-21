using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Response_Format
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [FormatFilter]
    public class ResponseFormatController : ControllerBase
    {
        [HttpGet("{id:long}")]
        public IActionResult Ok(long id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            return Ok(new Item() { Name = "111" });
        }
        [HttpGet]
        public IActionResult Problem()
        {
            return Problem("return a problem");
        }
        [HttpGet]
        public JsonResult GetResult()
        {
            return new JsonResult(new Item() { Name = "222" });
        }
        [HttpGet("{id:long}.{format?}")]
        public Item Format(long id)
        {
            return new Item() { Name = "123"};
        }
    }
    public class Item
    {
        public string Name { get; set; }
    }
}
