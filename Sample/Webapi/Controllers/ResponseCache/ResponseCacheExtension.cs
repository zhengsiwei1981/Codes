

using Microsoft.AspNetCore.Mvc;

namespace Webapi.MyExtension
{
    public static class ResponseCacheExtension
    {
        public static void SampleResponseCacheForBuilder(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.PostConfigure<MvcOptions>(options =>
            {
                options.CacheProfiles.Add("Default", new CacheProfile() { Duration = 10 });
            });
        }
        public static void SampleResponseCacheForWebApplication(this WebApplication app)
        {
            app.UseResponseCaching();
        }
    }
}
