using Newtonsoft.Json;

namespace NancyAPI.Models
{
    public class ErrorResponse
    {
        [JsonProperty("fault")]
        public Fault Fault { get; set; }
    }
}
