using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO.Abstractions;

namespace Webapi.Controllers.FileProvider
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileProviderController : ControllerBase
    {
        [HttpGet]
        public string GetRootContentPath([FromServices] IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.ContentRootPath + " : " + hostEnvironment.ContentRootFileProvider.GetType().ToString();
        }
        [HttpGet("fileCreate/{name}")]
        public IActionResult CreateFile(string name, [FromServices] IFileProvider fileProvider, [FromServices] IFileSystem fileSystem, [FromServices] IHostEnvironment hostEnvironment)
        {
            var file = fileProvider.GetFileInfo(name);
            if (!file.Exists)
            {
                fileSystem.FileStream.New(file.PhysicalPath, FileMode.CreateNew);
            }
            return Ok(file.Name);
        }
        /// <summary>
        ///  1. 将 Microsoft.Extensions.FileProviders.Embedded NuGet 包添加到项目中。
        ///  2. 将该属性设置为 。指定要嵌入<嵌入资源> 的文件：<GenerateEmbeddedFilesManifest>true
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileProvider"></param>
        /// <param name="fileSystem"></param>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        [HttpGet("GetEmbeddedFile/{name}")]
        public IActionResult GetEmbeded(string name, [FromServices] IFileProvider fileProvider, [FromServices] IFileSystem fileSystem, [FromServices] IHostEnvironment hostEnvironment)
        {
            var file = fileProvider.GetFileInfo(name);
            if (file.Exists)
            {
                using var stream = file.CreateReadStream();
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Ok(System.Text.Encoding.UTF8.GetString(bytes));
            }
            return NotFound(file.Name);
        }
        [HttpGet("Directory/{name}")]
        public IActionResult Directory(string name, [FromServices] IFileSystem fileSystem, [FromServices] ILogger<FileProviderController> logger)
        {
            var dir = fileSystem.DirectoryInfo.New(name);
            if (!dir.Exists)
            {
                dir.Create();
                var watcher = fileSystem.FileSystemWatcher.New(name, "*.*");
                watcher.Created += (sender, e) =>
                {
                    logger.LogInformation(e.FullPath);
                };
                watcher.Changed += (sender, e) =>
                {
                    logger.LogInformation(e.FullPath);
                };
                watcher.EnableRaisingEvents = true;
            }
            return Ok(dir.FullName);
        }
    }
}
