using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Webapi.Controllers.Logging
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        /// <summary>
        /// 这里的日志将不会写入
        /// </summary>
        /// <param name="info"></param>
        /// <param name="logger"></param>
        [HttpGet("warning/{info}")]
        public void Info(string info, [FromServices] ILogger<LoggingController> logger)
        {
            logger.LogInformation(info);
        }
        /// <summary>
        /// 这里的日志将会写入
        /// </summary>
        /// <param name="info"></param>
        /// <param name="logger"></param>
        [HttpGet("infomation/{info}")]
        public void Info2(string info, [FromServices] ILogger<DefaultLoggingLevel> logger)
        {
            logger.LogInformation(info);
        }
        /// <summary>
        /// 添加日志的上下文信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="logger"></param>
        [HttpGet("scope/{info}")]
        public void Info3(string info, [FromServices] ILogger<DefaultLoggingLevel> logger)
        {
            var transcationId = Guid.NewGuid().ToString();
            using (logger.BeginScope("transcationId"))
            {
                logger.LogInformation(new EventId(1000), "operation:{transcationId}", transcationId);
                if (!string.IsNullOrEmpty(info))
                {
                    logger.LogInformation(new EventId(1000), info);
                }
                else
                {
                    logger.LogWarning(new EventId(1001), "Not Found : {transcationId}", transcationId);
                }
            }
        }
    }
    public class DefaultLoggingLevel
    { }
}
