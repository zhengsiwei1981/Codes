using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Response_Format
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [FormatFilter]
    public class ResponseFormatController : ControllerBase
    {
        /// <summary>
        /// ProducesResponseType应用于存在多个相应类型时
        /// Produces固定相应的文本类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Produces("application/xml")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
