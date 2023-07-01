using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Webapi.Controllers.ModelBinder
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelBinderController : ControllerBase
    {
        /// <summary>
        /// 属性绑定
        /// </summary>
        [BindProperty(Name = "name", SupportsGet = true)]
        public string? Name { get; set; }
        /// <summary>
        /// 属性绑定
        /// 设置来源（设置来源的情况下，属性绑定将失效）
        /// </summary>
        //[BindProperty(Name = "val", SupportsGet = true)]
        [FromQuery]
        public string? Value { get; set; }
        //[HttpGet("{name}/{val}")]
        [HttpGet("{name}")]
        public string BindTest()
        {
            return string.Format(this.Name + "," + this.Value);
        }
        /// <summary>
        /// 绑定标头信息
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetAccept([FromHeader(Name = "accept")] string language)
        {
            return language;
        }
        [HttpPost("{petName}")]
        public string PostTest([FromRoute] string petName, Blob blob)
        {
            return string.Format($"{petName},{blob.Height},{blob.Width}");
        }
        /// <summary>
        /// 必须在Postman里手动给参数加上Newer.前缀
        /// Bind的第一个参数将指定绑定模型的哪些属性
        /// </summary>
        /// <param name="newer"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        public string CustomePrefix([FromQuery][Bind("Name", Prefix = "Newer")] Newer newer)
        {
            return string.Format($"{newer.Name},{newer.Age}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        [HttpGet("intarray")]
        [HttpPost("intarray")]
        public string GetIntArray([FromQuery] List<int> array)
        {
            var sb = new StringBuilder();
            array.ForEach(i =>
            {
                sb.Append(i);
            });
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [HttpGet("dictionary")]
        [HttpPost("dictionary")]
        public string GetDictionary([FromQuery] Dictionary<int, string> dictionary)
        {
            var sb = new StringBuilder();
            dictionary.ToList().ForEach(i =>
            {
                sb.Append(i.Value);
            });
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost("record")]
        public IActionResult Record(Person person)
        {
            return new JsonResult(person);
        }
        [HttpPost("File")]
        public string GetFileName(IFormFile formFile)
        {
       
            return formFile.FileName;
        }
    }
    public record Person(
    [Required] string Name, [Range(0, 150)] int Age, [BindNever] int Id);
    public class Blob
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }
    public class Newer
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
