using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.Net.Http.Headers;
namespace Webapi.MyExtension
{
    public static class CustomerFormatExtension
    {
        public static void SetCustomerFormatOption(this MvcOptions option)
        {
            //写入自定义格式
            option.InputFormatters.Insert(0, new MyTestInputFormatter());
            option.OutputFormatters.Insert(0, new MyTestOutputFormatter());
        }
    }
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
}
