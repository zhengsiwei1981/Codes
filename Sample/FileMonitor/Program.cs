using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO.Abstractions;
using System.IO;
using System.Collections.Concurrent;
using Microsoft.Win32.SafeHandles;
namespace FileMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args).ConfigureServices(services =>
            {
                services.AddSingleton<IFileSystem, FileSystem>();
                services.AddSingleton<FilesMonitor>(serviceProvider =>
                {
                    return new FilesMonitor("Test", serviceProvider.GetRequiredService<IFileSystem>());
                });
                services.AddHostedService<MonitorService>();
                services.AddHostedService<TestService>();
            }).Build().Run();
        }

    }
    public class TestService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        File.Create(Path.Combine(Directory.GetCurrentDirectory(), "Test", Guid.NewGuid().ToString() + ".txt")).Dispose();
                    }

                    break;
                    //await Task.Delay(5000);
                }
            }, stoppingToken);
        }
    }
    public class MonitorService : BackgroundService
    {
        private FilesMonitor filesMonitor;
        public MonitorService(FilesMonitor filesMonitor)
        {
            this.filesMonitor = filesMonitor;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.filesMonitor.DefaultMonitoringStart(stoppingToken);
            return Task.CompletedTask;
        }
    }
    public class FilesMonitor
    {
        private int _monitorcount;
        private ConcurrentQueue<FileSystemEventArgs> _files = new ConcurrentQueue<FileSystemEventArgs>();
        private bool IsOperation;
        private CancellationToken stoppingToken;
        /// <summary>
        /// 
        /// </summary>
        private readonly IFileSystem fileSystem;
        private string directoryName;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileSystem"></param>
        public FilesMonitor(string directoryName, IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            this.directoryName = directoryName;
            if (directoryName == null)
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            var dirFactory = fileSystem.DirectoryInfo.New(this.directoryName);
            if (!dirFactory.Exists)
            {
                dirFactory.Create();
            }
        }
        public void DefaultMonitoringStart(CancellationToken cancellationToken)
        {
            this.stoppingToken = cancellationToken;
            var dInfo = this.fileSystem.DirectoryInfo.New(this.directoryName);
            var watcher = dInfo.FileSystem.FileSystemWatcher.New(this.directoryName, "*.txt");
            watcher.Created += Watcher_Created1;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created1(object sender, FileSystemEventArgs e)
        {
            this.stoppingToken.ThrowIfCancellationRequested();
            SpinWait.SpinUntil(() => this.IsOperation == false);
            Interlocked.Add(ref _monitorcount, 1);
            _files.Enqueue(e);

            if (Interlocked.CompareExchange(ref _monitorcount, 0, 10) == 10)
            {
                lock (string.Intern("locked"))
                {
                    this.IsOperation = true;
                    _files.ToList().ForEach(e =>
                    {
                        var target = Path.Combine(Directory.GetCurrentDirectory(), "copy", e.Name!);
                        if (!File.Exists(target))
                        {
                            File.Copy(e.FullPath, target);
                            File.Delete(e.FullPath);
                        }
                    });

                    _files.Clear();
                    Console.WriteLine("copyFileCount:" + Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "copy")).Length);
                    Console.WriteLine("testFileCount:" + Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Test")).Length);
                    this.IsOperation = false;
                }
            }
        }
    }
}