using Newtonsoft.Json;

namespace NancyAPI.Core.Models
{
    public class ErrorResponse
    {
        [JsonProperty("fault")]
        public Fault Fault { get; set; }
    }
}
