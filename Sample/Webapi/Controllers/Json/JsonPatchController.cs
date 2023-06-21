using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Webapi.Controllers.Json
{
    /// <summary>
    /// JSON修补程序
    /// https://datatracker.ietf.org/doc/html/rfc6902
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JsonPatchController : ControllerBase
    {
        IMemoryCache memoryCache;
        public JsonPatchController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpPatch]
        public IActionResult JsonPatchWithModelState([FromBody] JsonPatchDocument<Customer> patchDoc)
        {
            var customer = CreateCustomer();
            patchDoc.ApplyTo(customer, ModelState);
            return new ObjectResult(customer);
        }
        private Customer CreateCustomer()
        {
            Customer customer;
            var hasValue = this.memoryCache.TryGetValue<Customer>("customer", out customer!);
            if (hasValue)
            {
                return customer;
            }
            else
            {
                return this.memoryCache.Set<Customer>("customer", new Customer() { CustomerName = "test", Orders = new List<Order>() });
            }
        }
    }

    public class Customer
    {
        public string? CustomerName { get; set; }
        public List<Order>? Orders { get; set; }
    }
    public class Order
    {
        public string? OrderName { get; set; }
        public string? OrderType { get; set; }
    }
}
