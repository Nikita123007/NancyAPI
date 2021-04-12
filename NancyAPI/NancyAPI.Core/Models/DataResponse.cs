using Newtonsoft.Json;
using System.Collections.Generic;

namespace NancyAPI.Core.Models
{
    public class DataResponse
    {
        [JsonProperty("results")]
        public List<ArticleSource> Results { get; set; }
    }
}
