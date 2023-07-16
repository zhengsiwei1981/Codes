using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapi.MyExtension;

namespace Webapi.Controllers.HttpClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpClientController : ControllerBase
    {
        [HttpGet("send")]
        public async Task<IActionResult> Send([FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("test1");
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "api/HttpClient/getmessage"));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return BadRequest();
            }
        }
        [Produces("application/json")]
        [HttpGet("sendbyservice")]
        public async Task<IActionResult> SendByService([FromServices] SampleHttpClientService sampleHttpClientService)
        {
            var data = await sampleHttpClientService.GetData();
            return Ok(data);
        }
        [HttpGet("retry")]
        public async Task<IActionResult> Retry([FromServices] IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient("pollytest1");
            HttpResponseMessage response;
            try
            {
                 response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "api/HttpClient/timeout"));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return BadRequest("fail");
            }
        }
        [HttpGet("getmessage")]
        public string GetMessage()
        {
            return "success";
        }
        [Produces("application/json")]
        [HttpGet("mydata")]
        public MyData GetMyData()
        {
            return new MyData() { Item1 = "1", Item2 = 2 };
        }
        [HttpGet("badrequest")]
        public IActionResult Bad()
        {
            return BadRequest();
        }
        [HttpGet("timeout")]
        public IActionResult Timeout()
        {
            Thread.Sleep(3000);
            return Ok();
        }
    }
}
