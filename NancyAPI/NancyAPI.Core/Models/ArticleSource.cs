using Newtonsoft.Json;
using System;

namespace NancyAPI.Core.Models
{
    public class ArticleSource
    {
        [JsonProperty("section")]
        public string Section { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("updated_date")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("short_url")]
        public string ShortUrl { get; set; }
    }
}
