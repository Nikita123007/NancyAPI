using Newtonsoft.Json;

namespace NancyAPI.Core.Models
{
    public class Fault
    {
        [JsonProperty("faultstring")]
        public string FaultString { get; set; }
    }
}
