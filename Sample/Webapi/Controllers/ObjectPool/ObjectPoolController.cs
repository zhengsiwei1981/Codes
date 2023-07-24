using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Webapi.Controllers.ObjectPool
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObjectPoolController : ControllerBase
    {
        [HttpGet("PoolString/{str}")]
        public IActionResult Sample(string str, [FromServices] ObjectPool<StringBuilder> objectPool)
        {
            //从池中获取一个对象，避免实例化，并且使对象不会被GC
            var sb = objectPool.Get();

            sb.Append(str);
            var v = sb.ToString();

            //将对象返回至池中
            objectPool.Return(sb);
            return Ok(v);
        }
    }
}
