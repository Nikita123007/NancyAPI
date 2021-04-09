using Newtonsoft.Json;
using System.Collections.Generic;

namespace NancyAPI.Models
{
    public class DataResponse
    {
        [JsonProperty("results")]
        public List<ArticleSource> Results { get; set; }
        public Fault Fault { get; set; }
    }
}
