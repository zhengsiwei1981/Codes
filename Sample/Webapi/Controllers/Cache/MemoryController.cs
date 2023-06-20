using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Webapi.Controllers.Cache
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        [HttpGet("{key}/{value}")]
        public void CacheSet(string key, string value, [FromServices] IDistributedCache distributedCache)
        {
            distributedCache.SetString(key, value);
        }
        [HttpGet]
        public string CacheGet(string key, [FromServices] IDistributedCache distributedCache)
        {
            return distributedCache.GetString(key)!;
        }
        [HttpGet("{key}/{value}/{timespan}")]
        public void CacheSet(string key, string value, double timespan, [FromServices] IDistributedCache distributedCache)
        {
            distributedCache.SetString(key, value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(timespan)));
        }
    }
}
