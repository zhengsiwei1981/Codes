using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.ResponseCache
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseCacheController : ControllerBase
    {
        /// <summary>
        /// 缓存30秒有效
        /// </summary>
        /// <returns></returns>
        [HttpGet("duration")]
        [ResponseCache(Duration = 30)]
        public IActionResult Cache()
        {
            return Ok(DateTime.Now.ToString());
        }
        // <summary>
        /// 根据查询参数缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet("varybyquery")]
        [ResponseCache(VaryByQueryKeys = new string[] { "key" }, Duration = 10000)]
        public IActionResult CacheByQuery(string key)
        {
            return Ok(DateTime.Now.ToString());
        }
        // <summary>
        /// 根据Header参数缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet("varybyHeader")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 10000)]
        public IActionResult CacheByHeader()
        {
            return Ok(DateTime.Now.ToString());
        }
        // <summary>
        /// 应用Profile
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        [ResponseCache(CacheProfileName = "Default")]
        public IActionResult CacheByProfile()
        {
            return Ok(DateTime.Now.ToString());
        }
        // <summary>
        /// 不缓存，通常用于错误页面
        /// Location.Any(Cache-Control设置为public)表示客户端或任何中间代理可以缓存该值，包括响应缓存中间件。
        /// Location.Client(Cache-Control设置为private) 表示只有客户端可以缓存该值。任何中间缓存都不应缓存该值，包括响应缓存中间件。
        /// </summary>
        /// <returns></returns>
        [HttpGet("noStore")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult NoStore()
        {
            return Ok(DateTime.Now.ToString());
        }
    }
}
