using EFTestModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Net;
using System.Text;
using System.Web.Http.ModelBinding;
using System.Xml;
using Webapi;
using Webapi.Controllers.Authorization;
using Webapi.Controllers.Configuration;
using Webapi.Controllers.HandlerError;
using Webapi.MyExtension;
using static System.Net.Mime.MediaTypeNames;

namespace Webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //Healthy Check
            builder.Services.SampleHealthyCheckForBuilder();
            //Cors
            builder.Services.SampleCorsForBuilder();
            //Http Client
            builder.Services.SampleHttpClientForBuilder();
            //Data Protection
            builder.Services.SampleDataProtection();
            //Object Pool
            builder.Services.SampleObjectPool();
            //Response Cache
            builder.Services.SampleResponseCacheForBuilder();
            //Session
            builder.Services.SampleSessionForBuilder();
            //Static File
            builder.Services.SampleStaticFileForBuilder();
            //Auth
            builder.Services.SampleAuthencticationForBuilder();
            //Authorizatioon
            builder.Services.SampleAuthorizationForService();
            //Logging
            builder.SampleLoggingForService();
            //Configuration
            builder.SampleConfigurationForService(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                //输出xml的格式
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                //写入自定义格式
                options.SetCustomerFormatOption();

                //options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(s => "值不能为空");
                //options.ModelBinderProviders.Insert(0, new ByteArrayModelBinderProvider());
                //添加异常过滤
                options.Filters.Add<HttpResponseExceptionFilter>();

            }).AddNewtonsoftJson(options =>
            {
                //设置日期格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd";
                //处理导航属性的循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //Json格式首字母大写
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //自定义转换器
                options.SerializerSettings.Converters.Add(new Webapi.Controllers.ModelBinder.DateTimeConverter());
            });
            builder.Services.Configure<PositionOptions>(builder.Configuration.GetSection("Postion"));

            builder.Services.AddSingleton<IFileProvider>(service =>
            {
                var env = service.GetRequiredService<IHostEnvironment>();
                var logger = service.GetRequiredService<Logger<Program>>();
                var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly);
                var physicalFileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "TestDocument"));

                return new CompositeFileProvider(physicalFileProvider, manifestEmbeddedProvider);
            });

            //定义模型规则
            builder.Services.SampleModelValidationForService();
           
            //测试用内存
            builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
            builder.Services.AddSingleton<IFileSystem, FileSystem>();
            //注册中间件
            //builder.Services.AddSingleton<IMiddleware, RequestFileMiddleware>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //注册默认版本号
            builder.Services.RegisteDefaultVersion();
            //注册默认分布式缓存
            builder.Services.RegisteDefaultCaching();
            //注册默认数据库ORM
            builder.Services.RegisteDefaultDataBase();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                //异常处理中间件
                //app.UseExceptionHandler(exceptionHandlerApp =>
                //{
                //    exceptionHandlerApp.Run(async context =>
                //    {
                //        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                //        // using static System.Net.Mime.MediaTypeNames;
                //        context.Response.ContentType = Text.Plain;

                //        await context.Response.WriteAsync("An exception was thrown.");

                //        var exceptionHandlerPathFeature =
                //            context.Features.Get<IExceptionHandlerPathFeature>();

                //        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                //        {
                //            await context.Response.WriteAsync(" The file was not found.");
                //        }

                //        if (exceptionHandlerPathFeature?.Path == "/")
                //        {
                //            await context.Response.WriteAsync(" Page: Home.");
                //        }
                //    });
                //});
            }

            //app.UseExceptionHandler("/error");

            //Rewriter
            app.SampleRewriteForApp();
            //添加Http日志
            //app.UseHttpLogging();
            //app.UseW3CLogging();
            app.UseHttpsRedirection();
            app.MapControllers();

            //Auth
            app.SampleAuthencticationForWebAplication();
            //Authorization
            app.SampleAuthorizationForBuilder();
            //static file
            app.SampleStaticFileForWebApplication();
            //Session
            app.UseSession();
            //Healthy Check
            app.SampleHealthyCheckForWebApplication();
            //Cors
            app.SampleCorsForWebApplication();
            //Response Cache
            app.SampleResponseCacheForWebApplication();


            //        app.Run(context => context.Response.WriteAsync(
            //$"Rewritten or Redirected Url: " +
            //$"{context.Request.Path + context.Request.QueryString}"));
            //app.UseRequestFileMiddleware();
            app.Run();
        }
    }

    #region 自定义格式化对象

    #endregion

    public static class Extension
    {
        /// <summary>
        /// 注册版本号
        /// </summary>
        /// <param name="services"></param>
        public static void RegisteDefaultVersion(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
        }
        /// <summary>
        /// 注册默认缓存
        /// </summary>
        /// <param name="services"></param>
        public static void RegisteDefaultCaching(this IServiceCollection services)
        {
            //Redis
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "r-uf6hs0hpgmtfflf6r5pd.redis.rds.aliyuncs.com:6379,password=r-uf6hs0hpgmtfflf6r5:6abeg79M";
            //    //options.Configuration = "r-uf6hs0hpgmtfflf6r5pd.redis.rds.aliyuncs.com:6379";
            //    options.InstanceName = "SampleInstance";
            //});

            //SqlServer
            //services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = "Data Source=.;Initial Catalog=Test;Integrated Security=True;TrustServerCertificate=true";
            //    options.SchemaName = "dbo";
            //    options.TableName = "TestCach";
            //});

            services.AddDistributedMemoryCache();
        }
        /// <summary>
        /// 注册默认ORM
        /// </summary>
        /// <param name="services"></param>
        public static void RegisteDefaultDataBase(this IServiceCollection services)
        {
            services.AddDbContext<EFTestModel.EFTestModel>(options =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestFileMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestFileMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var fileSystem = context.RequestServices.GetRequiredService<IFileSystem>();
            var fileInfo = fileSystem.FileInfo.New(Path.Combine(Directory.GetCurrentDirectory(), "1.txt"));

            var sb = new StringBuilder();
            foreach (var key in context.Request.Query.Keys)
            {
                var content = context.Request.Query[key].FirstOrDefault();
                sb.Append(content);
                sb.Append(Environment.NewLine);
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            using var stream = fileInfo.Open(FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);

            await _next(context);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class RequestFileMiddlewareUse
    {
        public static void UseRequestFileMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestFileMiddleware>();
        }
    }

    public class TextFileProvider : PhysicalFileProvider, IFileProvider
    {
        public TextFileProvider(string root) : base(root)
        { }
        public Microsoft.Extensions.FileProviders.IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = base.GetFileInfo(subpath);
            return fileInfo;
        }

    }
}