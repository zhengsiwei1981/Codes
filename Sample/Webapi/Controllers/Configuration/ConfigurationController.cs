using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Webapi.Controllers.Configuration
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet("providers")]
        public IActionResult GetProviders([FromServices] IConfiguration configuration)
        {
            string str = "";
            foreach (var provider in (configuration as IConfigurationRoot).Providers.ToList())
            {
                str += provider.ToString() + "\n";
            }
            return Content(str);
        }
        [HttpGet("simplereadpostion")]
        public IActionResult Get([FromServices] IConfiguration configuration)
        {
            var postion = configuration.GetSection("Postion").Get<PositionOptions>();
            return Ok(postion);

            //var list = configuration.GetSection("Postion").Get<List<string>>();
            //return Ok(list);

            //var dictionary = configuration.GetSection("Postion").Get<Dictionary<string, string>>();
            //return Ok(dictionary);

            //Tuple无法自动转换,缺少无参构造函数
            //var tuple = configuration.GetSection("Postion").Get<Tuple<string, string>>();
            //return Ok(tuple);
        }
        [HttpGet("postionpathread")]
        public IActionResult Get2([FromServices] IConfiguration configuration)
        {
            var obj = new { Foo = configuration["Postion:Foo"], Bar = configuration["Postion:Bar"] };
            return Ok(obj);
        }
        [HttpGet("bind")]
        public ContentResult OnGet([FromServices] IConfiguration configuration)
        {
            var positionOptions = new PositionOptions();
            configuration.GetSection("Postion").Bind(positionOptions);

            return Content($"Foo: {positionOptions.Foo} \n" +
                           $"Bar: {positionOptions.Bar}");
        }
        [HttpGet("array")]
        public IActionResult Getarray([FromServices] IConfiguration configuration)
        {
            var list = new List<Dictionary<string, string>>();
            var section = configuration.GetSection("myconfig");
            var children = section.GetChildren();
            foreach (var child in children)
            {
                var dic = child.Get<Dictionary<string, string>>();
                list.Add(dic);
            }
            return Ok(list);
        }
    }
    public class PositionOptions
    {
        public string Foo { get; set; } = string.Empty;
        public string Bar { get; set; } = string.Empty;
    }
}
