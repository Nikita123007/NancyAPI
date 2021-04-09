using Newtonsoft.Json;

namespace NancyAPI.Models
{
    public class Fault
    {
        [JsonProperty("faultstring")]
        public string FaultString { get; set; }
    }
}
