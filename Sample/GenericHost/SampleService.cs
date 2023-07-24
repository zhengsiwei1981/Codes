using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericHost
{
    internal class SampleService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    Console.WriteLine($"Count Time : {DateTime.Now.Millisecond}");
                    await Task.Delay(1000);
                }
            });
        }
    }
}
