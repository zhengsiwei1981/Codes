using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericHost
{
    internal class Program
    {
        /// <summary>
        ///主机是封装应用程序资源的对象，例如：
        ///依赖注入（DI）
        ///记录
        ///配置
        ///IHostedService实施
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);
            host.ConfigureServices(services =>
            {
                //注册服务
                services.AddHostedService<SampleService>();
            });
            host.Build().Run();
            //Console.ReadLine();
        }
    }
}