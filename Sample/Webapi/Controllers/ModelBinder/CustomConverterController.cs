using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Webapi.Controllers.ModelBinder
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomConverterController : ControllerBase
    {
        [HttpPost]
        [Produces("application/json")]
        public Test Converter(Test test)
        {
            return test;
        }
    }
    public class Test
    {
        public int Foo { get; set; }
        public string? TheDate { get; set; }
    }

    public class DateTimeConverter : Newtonsoft.Json.JsonConverter
    {

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Test);
        }

        public override object? ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var obj = (JObject)serializer.Deserialize(reader)!;
            DateTime dateTime;
            var d = obj.Value<string>("theDate");
            if (DateTime.TryParse(d, out dateTime))
            {
                return new Test() { Foo = obj.Value<int>("foo"), TheDate = dateTime.ToString("MM/dd/yyyy") };
            }
            else
            {
                return new Test() { Foo = obj.Value<int>("foo") };
            }
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            DateTime dateTime;
            var obj = value as Test;
            if (!DateTime.TryParse(obj.TheDate, out dateTime))
            {
                obj.TheDate = null;
            }
            writer.WriteRaw(JsonSerializer.Serialize(obj));
            serializer.Serialize(writer, obj);
        }
        //public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    DateTime dateTime;
        //    if (DateTime.TryParse(reader.GetString(), out dateTime))
        //    {
        //        return dateTime.ToString("MM/dd/yyyy");
        //    }
        //    return null;
        //}
        //public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        //{
        //    DateTime dateTime;
        //    if (DateTime.TryParse(value, out dateTime))
        //    {
        //        writer.WriteStringValue(dateTime.ToShortDateString());
        //    }
        //}
    }
}
