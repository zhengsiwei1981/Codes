using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.IO.Abstractions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

namespace Webapi.Controllers.Authorization
{
    public static class AuthorizationExtension
    {
        public static void SampleAuthorizationForService(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, CustomHandler>();
            services.AddAuthorization(option =>
            {
                option.AddPolicy("RequireAdmin", policy =>
                {
                    policy.RequireRole("admin");
                });
                option.AddPolicy("RequireName", policy =>
                {
                    policy.RequireUserName("zsw");
                });
                option.AddPolicy("ClaimCount", policy =>
                {
                    policy.AddRequirements(new CustomRequirement() { ClaimCount = 3 });
                });
            });

            //注册一个全局过滤来验证匿名访问
            //var policy = new AuthorizationPolicyBuilder()
            //         .RequireAuthenticatedUser()
            //         .Build();

            //services.PostConfigure<MvcOptions>(option =>
            //{
            //    option.Filters.Add(new AuthorizeFilter(policy));
            //});
        }
        public static void SampleAuthorizationForBuilder(this WebApplication app)
        {
            app.UseMiddleware<StaticResourceAuthorizationMiddleware>();
        }
    }
    public class CustomRequirement : IAuthorizationRequirement
    {
        public int ClaimCount { get; set; }
    }
    public class CustomHandler : AuthorizationHandler<CustomRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            if (context.User.Claims.Count() < requirement.ClaimCount)
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
    /// <summary>
    /// 中间件限制无权限的资源访问
    /// </summary>
    public class StaticResourceAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public StaticResourceAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string[] staticResourceExtension = { ".txt", ".png", ".jpg" };
            var pathValue = context.Request.Path.Value;
            if (pathValue.Contains("."))
            {
                var extension = Path.GetExtension(pathValue);
                if (staticResourceExtension.Contains(extension))
                {
                    if (!context.User.IsInRole("admin"))
                    {
                        await context.Response.WriteAsync("no authorization");
                        context.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
