using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Webapi.MyExtension
{
    /// <summary>
    /// 使用规则
    /// 1.必须在ResponseCaching中间件之前调用
    /// 2.必须在StaticFile中间件之前调用
    /// 
    /// 如果不指定规则无法使用跨域
    /// 必须在依赖注入组件和中间件上都注册了规则才能使用跨域，如果要为单独的终结点注册跨域，则不应该为中间件指定规则名称
    /// 如果在控制器的方法上使用DisableCors特性，则无论是否指定规则都无法使用跨域
    /// 如果在控制器的方法上使用EnableCors特性，则即使没有为中间件指定规则，都可以使用跨域
    /// </summary>
    public static class CorsExtension
    {
        public static void SampleCorsForBuilder(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("mypolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:7069");
                });
            });
        }
        public static void SampleCorsForWebApplication(this WebApplication app)
        {
            app.UseCors();
        }
    }
}
