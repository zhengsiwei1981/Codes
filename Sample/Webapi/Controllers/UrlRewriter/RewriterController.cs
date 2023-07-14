using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.IO.Abstractions;

namespace Webapi.Controllers.UrlRewriter
{
    /// <summary>
    /// 1.如果要使用文件路径的重写，必须启用静态文件中间件
    /// 2.必须在启用静态文件中间件之前启用重写规则
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class RewriterController : ControllerBase
    {
        [HttpGet("rewritten")]
        public string rewritten(string var1, string var2)
        {
            return var1 + var2;
        }
        [HttpGet("redirected/{v1}/{v2}")]
        public string redirected(string v1, string v2)
        {
            return this.HttpContext.Request.Path + this.HttpContext.Request.QueryString;
        }
    }
}
