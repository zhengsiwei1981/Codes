using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Webapi.Controllers.Route
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapPartController : ControllerBase
    {
        // GET: api/MapPart/1
        [HttpGet("{id}")]
        public string GetPart(int id)
        {
            return id.ToString();
        }
        // GET: api/MapPart/1/2
        [HttpGet("{id}/{age}")]
        public int GetPart2(int id, int age)
        {
            return id + age;
        }
        [HttpGet("detail/{id}/{age}/{height}")]
        public int GetPart3(int id, int age, int height)
        {
            return id + height + age;
        }
        // GET: api/MapPart?body=1
        [HttpPost]
        public string Detail(string body)
        {
            return body;
        }
        //api/MapPart/detail/1 body
        [HttpPost("detail/{id}")]
        public ActionResult Detail([FromBody] Body body, [FromRoute] int id)
        {
            return Content($"body weight:{body.Weight} id:{id}");
        }
        [HttpGet("header")]
        public ActionResult FromHeader([FromHeader] string accept)
        {
            return Content(accept);
        }
        //Get,Put,Delete
        [AcceptVerbs("Get", "Put", "Delete")]
        public int Methods(int id)
        {
            return 1;
        }
        public class Body
        {
            public double Weight { get; set; }
            public double Height { get; set; }
        }
    }


}
