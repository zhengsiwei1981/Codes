namespace Webapi.MyExtension
{
    /// <summary>
    /// 必须指定分布式缓存后才能应用会话
    /// </summary>
    public static class SessionExtension
    {
        public static void SampleSessionForBuilder(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                //会话超时
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });
        }
        /// <summary>
        /// 必须在UseRouting之后调用
        /// </summary>
        /// <param name="app"></param>
        public static void SampleSessionForWebApplication(this WebApplication app)
        {
            app.UseSession();
        }
    }
}
