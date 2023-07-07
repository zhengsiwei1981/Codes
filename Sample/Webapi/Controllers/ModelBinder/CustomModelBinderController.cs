using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Buffers;
using System.Text.Json;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Webapi.Controllers.ModelBinder
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomModelBinderController : ControllerBase
    {
        [HttpPost("Post")]
        public IActionResult Test([FromBody][Microsoft.AspNetCore.Mvc.ModelBinder(BinderType = typeof(TestObjModelBinder))] TestObj testObj)
        {
            return Content($"{testObj.Foo},{testObj.Bar}");
        }
        [HttpGet("Get")]
        public IActionResult Test2([FromQuery][Microsoft.AspNetCore.Mvc.ModelBinder(BinderType = typeof(TestObjModelBinder2))] TestObj testObj)
        {
            return Content($"{testObj.Foo},{testObj.Bar}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost("file")]
        public IActionResult FromFile([FromForm][Microsoft.AspNetCore.Mvc.ModelBinder(BinderType = typeof(ByteArrayModelBinder))] byte[] file, [FromForm] string fileName)
        {
            System.IO.File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), fileName), file);
            return Ok(fileName);
        }
    }

    public class TestObj
    {
        public string? Foo { get; set; }
        public string? Bar { get; set; }
    }
    public class TestObjModelBinder : Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder
    {
        public async Task BindModelAsync(Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext bindingContext)
        {
            try
            {
                var value = await bindingContext.ActionContext.HttpContext.Request.BodyReader.ReadAsync();
                var s = System.Text.Encoding.UTF8.GetString(value.Buffer.ToArray());
                var model = JsonConvert.DeserializeObject<TestObj>(s);

                model.Foo = model.Foo == "string" ? "default" : model.Foo;
                model.Bar = model.Bar == "string" ? "default" : model.Bar;

                bindingContext.Result = ModelBindingResult.Success(model);
            }
            catch (Exception exception)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    exception,
                    bindingContext.ModelMetadata);
            }
        }
    }
    public class TestObjModelBinder2 : Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder
    {
        public Task BindModelAsync(Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext bindingContext)
        {
            var model = Activator.CreateInstance(bindingContext.ModelType);
            bindingContext.ModelMetadata.Properties.ToList().ForEach(p =>
            {
                var valueProvider = bindingContext.ValueProvider.GetValue(p.Name);
                model.GetType().GetProperty(p.Name).SetValue(model, valueProvider.FirstValue);
            });
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }

}
