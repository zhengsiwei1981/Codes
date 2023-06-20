using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Webapi.Controllers.Cache
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        /// <summary>
        /// 1.注册阿里云
        /// 2.建立redis的实例
        /// 2.添加白名单
        /// 3.添加公有网络访问
        /// 4.在程序内添加访问连接
        /// </summary>
        /// <param name="distributedCache"></param>
        [HttpGet]
        public void Index([FromServices] IDistributedCache distributedCache)
        {
            distributedCache.Set("test3", System.Text.Encoding.UTF8.GetBytes("fefefewfew"));
            //var d = distributedCache.GetString("test");
        }
    }
}
