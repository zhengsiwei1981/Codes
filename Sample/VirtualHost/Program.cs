using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.Net;
using System.Runtime.CompilerServices;
using static VirtualHost.Program;

namespace VirtualHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestInterface, TestObject>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddLogging(builder => builder.AddConsole());

            var app = new ListeningApplicationBuilder().Build(services);
            app.Use(context =>
            {
                context.HttpListenerContext.Response.OutputStream.Write(System.Text.Encoding.UTF8.GetBytes("Hello Word"));
                context.ScopeProvider.GetRequiredService<ITestInterface>().Test();
            });
            app.Use(context =>
            {
                new MyController().Invoke(context);
            });
            app.Run();

            Console.ReadLine();
        }
    }

    public class ListeningApplication
    {
        public IServiceCollection Services { get; set; }
        public IList<Action<HttpContextWraper>> Middlewares { get; private set; }
        public ListeningApplication(IServiceCollection services)
        {
            //this.HttpContextWraper = wraper;
            this.Middlewares = new List<Action<HttpContextWraper>>();
            Services = services;
        }
        public void Use(Action<HttpContextWraper> middleware)
        {
            this.Middlewares.Add(middleware);
        }

        public async void Run()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();

            while (true)
            {
                var context = await listener.GetContextAsync();
                var httpContextWraper = HttpContextWraper.Build(context, this.Services);
                httpContextWraper.ScopeProvider.GetRequiredService<ILogger<ListeningApplication>>().LogInformation("received request");

                foreach (var middleware in this.Middlewares.Reverse())
                {
                    middleware(httpContextWraper);
                }
                httpContextWraper.HttpListenerContext.Response.Close();
            }
        }
    }
    public class ListeningApplicationBuilder
    {
        public ListeningApplication Build(IServiceCollection services)
        {
            return new ListeningApplication(services);
        }
    }
    public class HttpContextWraper
    {
        public HttpListenerContext HttpListenerContext { get; set; }
        public IServiceProvider ScopeProvider { get; set; }
        public HttpContextWraper(HttpListenerContext httpListenerContext, IServiceProvider serviceProvider)
        {
            this.HttpListenerContext = httpListenerContext;
            this.ScopeProvider = serviceProvider;
        }

        public static HttpContextWraper Build(HttpListenerContext httpListenerContext, IServiceCollection serviceCollection)
        {
            return new HttpContextWraper(httpListenerContext, serviceCollection.BuildServiceProvider().CreateScope().ServiceProvider);
        }
    }
    public interface ITestInterface
    {
        public void Test();
    }
    public interface IController
    {
        public void Invoke(HttpContextWraper httpContextWraper);
    }
    public class TestObject : ITestInterface
    {
        public TestObject() { }

        public void Test()
        {
            Console.WriteLine("implement TestObject1");
        }
    }
    public class MyController : IController
    {
        public HttpContextWraper HttpContextWraper { get; set; }
        public void Test([FromServices] ILogger<MyController> logger)
        {
            logger.LogInformation("Test was invoked");
            //this.HttpContextWraper.HttpListenerContext.Response.Headers[System.Net.HttpRequestHeader.AcceptEncoding] = "UTF-8";
            this.HttpContextWraper.HttpListenerContext.Response.ContentType = "text/html;charset=utf-8";
            this.HttpContextWraper.HttpListenerContext.Response.OutputStream.Write(System.Text.Encoding.UTF8.GetBytes("--实现了Controller的方法"));
        }
        public void Invoke(HttpContextWraper httpContextWraper)
        {
            this.HttpContextWraper = httpContextWraper;
            var methodName = httpContextWraper.HttpListenerContext.Request.Url?.Segments.Last();
            if (methodName != null)
            {
                var method = this.GetType().GetMethod(methodName);
                if (method != null)
                {
                    var parameters = method.GetParameters().ToList();
                    var objects = new List<object>();
                    parameters.ForEach(p =>
                    {
                        if (p.IsDefined(typeof(FromServicesAttribute), false))
                        {
                            //var obj = httpContextWraper.ScopeProvider.GetType().GetMethod("GetRequiredService")!.MakeGenericMethod(new Type[] { p.ParameterType }).Invoke(httpContextWraper.ScopeProvider, null);
                            var obj = httpContextWraper.ScopeProvider.GetRequiredService(p.ParameterType);
                            if (obj != null)
                            {
                                objects.Add(obj);
                            }
                        }
                    });
                    method.Invoke(this, objects.Count > 0 ? objects.ToArray() : null);
                }
            }
        }
    }
}