using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Webapi.MyExtension
{
    public static class AuthencticationExtension
    {
        public static void SampleAuthencticationForBuilder(this IServiceCollection services)
        {
            // JWT的设置
            //var provider = services.BuildServiceProvider();
            //var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            //services.Configure<Token>(configuration.GetSection("Token"));

            //var option = configuration.GetSection("Token").Get<Token>();
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //{
            //    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            //    {
            //        ValidateIssuer = true,//是否在令牌期间验证签发者
            //        ValidateAudience = true,//是否验证接收者
            //        ValidateLifetime = true,//是否验证失效时间
            //        ValidateIssuerSigningKey = true,//是否验证签名
            //        ValidAudience = option.Audience,
            //        ValidIssuer = option.Issuer,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.SecretKey))
            //    };
            //});

            //Cookie的设置

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                option.LoginPath = "/api/Cookies/DenyAccess";             
                option.AccessDeniedPath= "/api/Authorization/AccessDenied";
            });
        }
        public static void SampleAuthencticationForWebAplication(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
    public class Token
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
    }
}
