using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Webapi.Controllers.HandlerError
{
    [Route("/error")]
    [ApiController]
    public class HandlerErrorController : ControllerBase
    {
        /// <summary>
        /// 三种方式全局捕获异常信息：
        /// 1.定义错误句柄
        /// 2.使用过滤器
        /// 3.使用错误中间件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Error()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
            return Problem(detail: exceptionHandlerFeature.Error.StackTrace, title: exceptionHandlerFeature.Error.Message);
        }
        [HttpGet("Throw")]
        public IActionResult Throw()
        {
            throw new Exception();
        }
    }
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.Result = new ObjectResult(context.Exception?.Message)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };

            context.ExceptionHandled = true;
        }
    }
}
