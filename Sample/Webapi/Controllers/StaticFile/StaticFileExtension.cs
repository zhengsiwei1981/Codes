using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Drawing;

namespace Webapi.MyExtension
{
    public static class StaticFileExtension
    {
        public static void SampleStaticFileForBuilder(this IServiceCollection services)
        {
            services.AddOptions<StaticFileOptions>().Configure<IWebHostEnvironment>((option, env) =>
            {
                //为所有的png图片增加水印
                option.FileProvider = new MyFileProvider(env.ContentRootPath);
            });
        }
        public static void SampleStaticFileForWebApplication(this WebApplication app)
        {
            app.UseStaticFiles();
        }
    }
    public class MyFileProvider : PhysicalFileProvider, IFileProvider
    {
        public static string intentString = null;
        public MyFileProvider(string root) : base(root)
        {
        }
        private void AddTextWatermark(string sourceImage, string watermarkText, FileInfo fileInfo)
        {
            var tempFileName = fileInfo.FullName.Replace(fileInfo.Name, Guid.NewGuid().ToString());
            using (var bitmap = System.Drawing.Image.FromFile(sourceImage))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawString(watermarkText, new Font("Arial", 12), new SolidBrush(System.Drawing.Color.Black), new System.Drawing.PointF(50, 50));
                    bitmap.Save(tempFileName);
                }
            }

            File.Replace(tempFileName, sourceImage, null);
            File.Delete(tempFileName);
        }
        IFileInfo IFileProvider.GetFileInfo(string subpath)
        {
            subpath = subpath.TrimStart('/');
            var fullPath = Path.GetFullPath(Path.Combine(Root, subpath));
            var FileInfo = new FileInfo(fullPath);
            if (!FileInfo.Exists)
                throw new FileNotFoundException(fullPath);
            
            if (FileInfo.Extension == ".png")
            {
                if (string.IsInterned(FileInfo.Name) == null)
                {
                    string.Intern(FileInfo.Name);
                }
                //var isEques = MyFileProvider.intentString == FileInfo.Name;
                var intenString = String.IsInterned(FileInfo.Name);
                var isEquals = FileInfo.Name.Equals(intenString);

                if (!FileMemories.fileBytes.ContainsKey(FileInfo.Name))
                {
                    lock (FileInfo.Name)
                    {
                        if (!FileMemories.fileBytes.ContainsKey(FileInfo.Name))
                        {
                            AddTextWatermark(fullPath, "TestCode", FileInfo);

                            using (var fs = FileInfo.OpenRead())
                            {
                                var bytes = new byte[fs.Length];
                                fs.Read(bytes, 0, bytes.Length);

                                var copybytes = new byte[bytes.Length];
                                Array.Copy(bytes, copybytes, bytes.Length);
                                FileMemories.fileBytes.TryAdd(FileInfo.Name, copybytes);
                            }
                        }
                    }
                }
                return new MyFileInfo(FileMemories.fileBytes[FileInfo.Name]);
            }
            else
            {
                return base.GetFileInfo(subpath);
            }
        }
    }
    public static class FileMemories
    {
        public static readonly ConcurrentDictionary<string, byte[]> fileBytes = new ConcurrentDictionary<string, byte[]>();
    }
    public class MyFileInfo : IFileInfo
    {
        private byte[] bytes = null;
        public MyFileInfo(byte[] _bytes)
        {
            this.bytes = _bytes;
        }
        public bool Exists => true;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified => DateTime.Now;

        public long Length => this.bytes.Length;

        public string Name => "MyTxtName";

        public string PhysicalPath => "";

        public Stream CreateReadStream()
        {
            return new MemoryStream(this.bytes, 0, this.bytes.Length);
        }
    }
}
