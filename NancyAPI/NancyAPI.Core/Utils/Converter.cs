using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NancyAPI.Core.Utils
{
    public static class Converter
    {
        private static JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string ToJson(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T FromJson<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings);
        }
    }
}
