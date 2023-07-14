using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Webapi.Controllers.Configuration;

namespace Webapi.Controllers.Options
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        /// <summary>
        /// 使用 IPostConfigureOptions<TOptions> 设置配置后。配置后在所有 IConfigureOptions<TOptions> 配置发生后运行：
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet("PositionOptionGet")]
        public ContentResult OnGet([FromServices] IOptions<PositionOptions> options)
        {
            options.Value.Foo = "myfoobar2";
            return Content(options.Value.Foo + options.Value.Bar);
        }
        /// <summary>
        /// options的值可以在运行时变更，但是作用于服务域的快照将只会加载配置的信息并生成一个临时对象
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet("SnapShot")]
        public ContentResult OnGet([FromServices] IOptionsSnapshot<PositionOptions> options)
        {
            return Content(options.Value.Foo + options.Value.Bar);
        }
        /// <summary>
        /// 可监视options的变更
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        [HttpGet("Monitor")]
        public ContentResult OnGet([FromServices] IOptionsMonitor<PositionOptions> options1)
        {
            options1.OnChange(options =>
            {
                Console.WriteLine(options.Foo);
            });
            return Content(options1.CurrentValue.Foo + options1.CurrentValue.Bar);
        }
    }
}
