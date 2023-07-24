using GenericHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace GenericHost
{
    public class Program
    {
        /// <summary>
        ///主机是封装应用程序资源的对象，例如：
        ///依赖注入（DI）
        ///记录
        ///配置
        ///IHostedService实施
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            //预处理
            //设置默认内容目录为当前文件夹目录
            //设置获取前缀为DOTNET的环境变量
            //设置获取命令行参数
            //设置获取appsettings.json和与当前环境变量定义的环境同名的配置文件
            //当环境变量是Development时从User secret加载配置
            //加载日志组件
            //当环境变量是Development时依赖注入组件将启用范围验证
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices(services =>
            {
                //services.AddSingleton<ILogger, MyLogger>();
                //注册服务
                services.AddHostedService<SampleService>();
                //services.AddHostedService<RaiseErrorHandler>();
                //services.AddHostedService<RaiseAsyncErrorHandler>();
            });
            //添加配置文件
            hostBuilder.ConfigureHostConfiguration(options =>
            {
                options.AddJsonFile("host.json", true);
            });
            hostBuilder.ConfigureHostOptions(options =>
            {
                //设置5分钟超时，主机触发ApplicationStopping停止服务
                //options.ShutdownTimeout = TimeSpan.FromSeconds(5);
            });
            //清除日志输出
            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                //添加自定义日志
                logging.AddProvider(new MyLoggerProvider());
                //logging.AddConsole();
            });

            var host = hostBuilder.Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("111");

            //查看基础环境
            var env = host.Services.GetRequiredService<IHostEnvironment>();
            Console.WriteLine($"EnvironmentName:{env.EnvironmentName},ContentRootPath : {env.ContentRootPath}, ApplicationName : {env.ApplicationName}");

            //HostTime组件将跟踪主机的生命周期,ConsoleLifetime是IHostLifeTime的默认实现
            var hostTime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            hostTime.ApplicationStarted.Register(() => { Console.WriteLine("Started"); });
            hostTime.ApplicationStopping.Register(() => { Console.WriteLine("Stopping"); });
            hostTime.ApplicationStopped.Register(() => { Console.WriteLine("Stopped"); });

            //读取配置信息
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            //读取全部配置
            //foreach (var config in configuration.AsEnumerable())
            //{
            //    Console.WriteLine($"Key:{config.Key}, Value:{config.Value}");
            //}
            Console.WriteLine($"Host Config Infomation:{configuration["Sample"]}");
            Console.WriteLine($"User Secret Infomation:{configuration["Secret Sample"]}");
            Console.WriteLine($"Environment Infomation:{configuration["DOTNET_MYENVVAL"]}");

            //通过应用程序域捕获未处理的异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //将会捕获每一层代码抛出的异常
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            //捕获异步线程中未处理的异常，只对异步线程有效(无法生效)
            //TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
            host.Run();

            //Console.ReadLine();
        }

        private static void UnobservedTaskExceptionHandler(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            //这是个根异常，代表异步线程可能返回多个异常，将会被全部保存在innerExceptions中
            var aggregate = e.Exception;
            Console.WriteLine("Unobserved Raise Exception:", aggregate.Message);
        }

        private static void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine("FirstChance Raise Exception：" + e.Exception.Message);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Console.WriteLine("Unhandled Raise Exception:  " + exception.Message);
        }
    }
}