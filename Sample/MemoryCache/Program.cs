using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace MemoryCache
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new ServiceCollection();
            service.AddMemoryCache();
            service.AddSingleton<IFileProvider>(provider =>
            {
                return new PhysicalFileProvider(Environment.CurrentDirectory);
            });

            var provider = service.BuildServiceProvider();
            var cache = provider.GetRequiredService<IMemoryCache>();


            cache.Set("filedata", getFileContent(provider), CreateOptions(provider));

            while (true)
            {
                var val = cache.Get<string>("filedata");
                Console.WriteLine(val);
                Thread.Sleep(1000);
            }
        }
        private static MemoryCacheEntryOptions CreateOptions(ServiceProvider serviceProvider)
        {
            var cache = serviceProvider.GetRequiredService<IMemoryCache>();
            var options = new MemoryCacheEntryOptions();
            var fileProvider = serviceProvider.GetRequiredService<IFileProvider>();

            options.AddExpirationToken(fileProvider.Watch("TextFile.txt"));
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                Console.WriteLine($"{key} {value} {reason} {state}");
                cache.Set(key, getFileContent(serviceProvider), CreateOptions(serviceProvider));
            });

            return options;
        }
        private static string getFileContent(ServiceProvider serviceProvider)
        {
            var fileProvider = serviceProvider.GetRequiredService<IFileProvider>();
            var fileInfo = fileProvider.GetFileInfo("TextFile.txt");
            using var stream = fileInfo.CreateReadStream();
            using var sr = new StreamReader(stream);
            return sr.ReadLine();
        }
    }
}