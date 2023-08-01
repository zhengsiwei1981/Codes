using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Webapi.MyExtension
{
    public static class RewriteExtension
    {
        public static void SampleRewriteForService(this IServiceCollection services)
        {
            services.Configure<RewriteOptions>(options =>
            {
                //使用自定义的重写规则，除此方法外还可以使用IRule实现相同的功能
                //options.Add(MethodRules.RewriteTextFileRequests);

                options.AddRedirect("redirect-rule/(.*)", "api/Rewriter/redirected/$1");
                //尽可能使用，因为匹配规则的计算成本很高，并且会增加应用响应时间。发生匹配时跳过对其余规则的处理，并且不需要其他规则处理
                options.AddRewrite(@"^sample/(\d+)/(\d+)", "api/Rewriter/rewritten?var1=$1&var2=$2", skipRemainingRules: true);
            });
        }
        public static void SampleRewriteForApp(this WebApplication app)
        {
            app.UseRewriter();
        }
    }
    public class MethodRules
    {
        #region snippet_RedirectXmlFileRequests
        public static void RedirectXmlFileRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            // Because the client is redirecting back to the same app, stop 
            // processing if the request has already been redirected.
            if (request.Path.StartsWithSegments(new PathString("/xmlfiles")) ||
                request.Path.Value == null)
            {
                return;
            }

            if (request.Path.Value.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                var response = context.HttpContext.Response;
                response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                context.Result = RuleResult.EndResponse;
                response.Headers[HeaderNames.Location] =
                    "/xmlfiles" + request.Path + request.QueryString;
            }
        }
        #endregion

        #region snippet_RewriteTextFileRequests
        public static void RewriteTextFileRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            if (request.Path.Value != null &&
                request.Path.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RuleResult.SkipRemainingRules;
                request.Path = "/file.txt";
            }
        }
        #endregion
    }
}
