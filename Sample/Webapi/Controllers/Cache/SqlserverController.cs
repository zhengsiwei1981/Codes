using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Webapi.Controllers.Cache
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SqlserverController : ControllerBase
    {
        /// <summary>
        /// 1.创建sql cache工具：dotnet tool install --global dotnet-sql-cache
        /// 2.创建缓存表 dotnet sql-cache create "Data Source=.;Initial Catalog=Test;Integrated Security=True;TrustServerCertificate=true" dbo TestCache
        /// 3.在程序内添加访问连结
        /// </summary>
        /// <param name="distributedCache"></param>
        [HttpGet]
        public void Index([FromServices] IDistributedCache distributedCache)
        {
            distributedCache.Set("test3", System.Text.Encoding.UTF8.GetBytes("fefefewfew "));
        }
    }
}
