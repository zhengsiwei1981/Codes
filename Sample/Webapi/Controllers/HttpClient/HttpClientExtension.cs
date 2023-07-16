using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Timeout;
using System.Net;
namespace Webapi.MyExtension
{
    public static class HttpClientExtension
    {
        public static void SampleHttpClientForBuilder(this IServiceCollection services)
        {
            services.AddPolicyRegistry();
            services.AddTransient<SampleHeaderHandler>();
            services.TryAddScoped<IOperationScoped, OperationScoped>();
            services.AddHttpClient("test1", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7036");
            }).AddHttpMessageHandler<SampleHeaderHandler>();

            services.AddHttpClient("pollytest1", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7036");
            }).AddTransientHttpErrorPolicy(policyBuilder =>
            {
                policyBuilder.OrResult(msg => msg.StatusCode != HttpStatusCode.OK)
                    .Or<TimeoutRejectedException>()
                    .Or<TaskCanceledException>()
                    .Or<OperationCanceledException>();
                //重试3次，每次间隔600毫秒
                return policyBuilder.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromMilliseconds(600), (req, dealy) =>
                {
                    Console.WriteLine($"req:{req.Result}");
                });
            }).AddPolicyHandler(req =>
            {
                //2秒超时
                return Policy.TimeoutAsync<HttpResponseMessage>(2);
            });
            services.AddHttpClient<SampleHttpClientService>();

        }
    }
    public class SampleHttpClientService
    {
        private readonly HttpClient _httpClient;
        public SampleHttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7036");
        }
        public async Task<MyData> GetData()
        {

            var data = await _httpClient.GetFromJsonAsync<MyData>("api/HttpClient/mydata");
            return data;
        }

    }
    public class ValidateHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("X-API-KEY"))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(
                        "The API key header X-API-KEY is required.")
                };
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
    public class SampleHeaderHandler : DelegatingHandler
    {
        private IOperationScoped _operationScoped;
        public SampleHeaderHandler(IOperationScoped operationScoped)
        {
            _operationScoped = operationScoped;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("SCOPED_ID", _operationScoped.OperationId);
            return await base.SendAsync(request, cancellationToken);
        }
    }
    public interface IOperationScoped
    {
        string OperationId { get; }
    }

    public class OperationScoped : IOperationScoped
    {
        public string OperationId { get; } = Guid.NewGuid().ToString()[^4..];
    }
    public class MyData
    {
        public string Item1
        {
            get; set;
        }
        public int Item2
        {
            get; set;
        }
    }
}
