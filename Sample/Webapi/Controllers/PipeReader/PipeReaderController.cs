using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Webapi.Controllers.PipeReader
{
    [Route("api/[controller]")]
    [ApiController]
    public class PipeReaderController : ControllerBase
    {
        private readonly ILogger<PipeReaderController> _logger;
        public PipeReaderController(ILogger<PipeReaderController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 适用于小文件的对象上传
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<string> GetFileList(IFormFile formFile)
        {
            var str = "";
            var formFiles = await this.HttpContext.Request.ReadFormAsync();
            using (var stream = formFiles.Files[0].OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    str = reader.ReadToEnd();
                }
            }
            return str;
        }
        /// <summary>
        /// 管道读取，此方法将会读取请求块的流数据，不仅限于文件，也包括以其他类型上传的数据
        /// 使用postman上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost("piperead")]
        [Produces("application/json")]
        public async Task<List<string>> ReadByPipe()
        {
            var data = new List<string>();
            while (true)
            {
                var readResult = await this.HttpContext.Request.BodyReader.ReadAsync();
                var buffer = readResult.Buffer;
                SequencePosition? position = null;
                do
                {
                    position = buffer.PositionOf((byte)'\n');
                    if (position != null)
                    {
                        var span = buffer.Slice(0, position.Value);

                        var str = System.Text.Encoding.UTF8.GetString(span.ToArray());
                        data.Add(str);

                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }

                }
                while (position != null);

                this.HttpContext.Request.BodyReader.AdvanceTo(buffer.Start, buffer.End);

                if (readResult.IsCompleted)
                {
                    break;
                }
            }

            return data;
        }
        /// <summary>
        ///大文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(UploadLargeFile))]
        public async Task<IActionResult> UploadLargeFile()
        {
            var request = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    // Get the temporary folder, and combine a random file name with it
                    var fileName = Path.GetRandomFileName();
                    var saveToPath = Path.Combine(Path.GetTempPath(), fileName);

                    using (var targetStream = System.IO.File.Create(saveToPath))
                    {
                        await section.Body.CopyToAsync(targetStream);
                    }

                    return Ok();
                }

                section = await reader.ReadNextSectionAsync();
            }

            // If the code runs to this location, it means that no files have been saved
            return BadRequest("No files data in the request.");
        }
    }
}
