using System.Diagnostics;

namespace Webapi.MyExtension
{
    public static class LoggingExtension
    {
        public static void SampleLoggingForService(this WebApplicationBuilder builder)
        {
            //builder.Services.AddW3CLogging(options =>
            //{
            //    options.FileName = "MyLogFile";
            //    options.LogDirectory = @"C:\Logs";
            //});
            builder.Logging.AddTraceSource(new System.Diagnostics.SourceSwitch("default", "all"), new DefaultTraceListener() { LogFileName = "trace.log" }).AddEventSourceLogger();
        }
    }
}
