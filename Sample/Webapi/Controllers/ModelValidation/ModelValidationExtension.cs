using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Webapi.MyExtension
{
    public static class ModelValidationExtension
    {
        public static void SampleModelValidationForService(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //禁用默认的无效模型响应器,否则自定义的模型验证将不会起效
                //options.SuppressModelStateInvalidFilter = true;

                //自定义无效模型响应
                options.InvalidModelStateResponseFactory = context =>
                {
                    var sb = new StringBuilder();
                    foreach (var key in context.ModelState.Keys)
                    {
                        if (context.ModelState[key].Errors.Count > 0)
                        {
                            sb.Append(context.ModelState[key].Errors[0].ErrorMessage);
                            sb.Append(",");
                        }
                    }
                    sb = sb.Remove(sb.Length - 1, 1);
                    return new JsonResult(new { message = $"无效的参数:{sb.ToString()}" });
                };
            });
        }
    }
}
