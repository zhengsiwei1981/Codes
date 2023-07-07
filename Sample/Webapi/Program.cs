using EFTestModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO.Abstractions;
using System.Text;
using System.Web.Http.ModelBinding;
using System.Xml;
using Webapi;
using Webapi.Controllers.HandlerError;
using static System.Net.Mime.MediaTypeNames;

namespace Webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                //输出xml的格式
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

                //写入自定义格式
                options.InputFormatters.Insert(0, new MyTestInputFormatter());
                options.OutputFormatters.Insert(0, new MyTestOutputFormatter());
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(s => "值不能为空");
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

            builder.Services.Configure<ApiBehaviorOptions>(options =>
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

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            //app.UseRequestFileMiddleware();
            app.Run();
        }
    }

    #region 自定义格式化对象
    public class MyTestInputFormatter : TextInputFormatter
    {
        public MyTestInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/mytest"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            try
            {
                using var reader = new StreamReader(context.HttpContext.Request.Body, encoding);
                var sb = new StringBuilder();
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<MyTest>>();
                var line = await ReadLineAsync("Name:", reader, context, logger);
                sb.Append(line);
                var line2 = await ReadLineAsync("Value:", reader, context, logger);
                sb.Append(line2);

                logger.LogInformation(sb.ToString());

                return await InputFormatterResult.SuccessAsync(new MyTest() { Name = line.Replace("Name:", ""), Value = line2.Replace("Value:", "") });
            }
            catch
            {
                return await InputFormatterResult.FailureAsync();
            }
        }
        private static async Task<string> ReadLineAsync(string expectedText, StreamReader reader, InputFormatterContext context, ILogger logger)
        {
            var line = await reader.ReadLineAsync();

            if (line is null || !line.StartsWith(expectedText))
            {
                var errorMessage = $"Looked for '{expectedText}' and got '{line}'";
                //context.ModelState.TryAddModelError(context.ModelName, errorMessage);
                logger.LogError(errorMessage);

                throw new Exception(errorMessage);
            }

            return line;
        }
    }
    public class MyTestOutputFormatter : TextOutputFormatter
    {
        public MyTestOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/mytest"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override bool CanWriteType(Type? type)
        {
            return type == typeof(MyTest) || typeof(IEnumerable<MyTest>).IsAssignableFrom(type); ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="selectedEncoding"></param>
        /// <returns></returns>
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.Object is MyTest)
            {
                var obj = context.Object as MyTest;
                string val = "no data";
                if (obj != null)
                {
                    val = $"Name:{obj.Name},Value:{obj.Value}";
                }
                await context.HttpContext.Response.WriteAsync(val);
            }
            else
            {
                var obj = context.Object as IEnumerable<MyTest>;
                var sb = new StringBuilder();
                foreach (var test in obj!)
                {
                    sb.Append($"Name:{test.Name},Value:{test.Value}");
                    sb.Append(Environment.NewLine);
                }
                await context.HttpContext.Response.WriteAsJsonAsync(sb.ToString());
            }
        }
    }
    public class MyTest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }
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
}