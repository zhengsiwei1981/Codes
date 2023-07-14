using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.CommandLine;
using System.Text;

namespace Webapi.Controllers.Envrionment
{
    /// <summary>
    /// DOTNET_ENVIRONMENT 用来定义非web应用
    /// ASPNETCORE_ENVIRONMENT 用来定义web应用
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EnvController : ControllerBase
    {
        [HttpGet("env")]
        public string GetEnvName([FromServices] IWebHostEnvironment environment)
        {
            return environment.EnvironmentName;
        }
        [HttpGet("arg")]
        public string GetEnvArgs([FromServices] IConfiguration configuration)
        {
            var sb = new StringBuilder();
            foreach (var arg in configuration.AsEnumerable())
            {
                sb.Append(arg.Key + ":" + arg.Value + Environment.NewLine);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 包含对环境变量的读取规则
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpGet("arg/{name}")]
        public string GetEnvArgs2(string name, [FromServices] IConfiguration configuration)
        {
            return configuration[name];
        }
        [HttpGet("commandLine")]
        public string GetCommandLine()
        {
            return Environment.GetCommandLineArgs().Aggregate((a, b) =>
            {
                return a + Environment.NewLine + b;
            });
        }
    }
}
