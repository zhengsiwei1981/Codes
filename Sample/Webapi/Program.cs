using EFTestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Webapi;
namespace Webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                //处理导航属性的循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
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
        public static void RegisteDefaultDataBase(this IServiceCollection services)
        {
            services.AddDbContext<EFTestModel.EFTestModel>(options =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
        }
    }

}