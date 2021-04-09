using System;

namespace NancyAPI.Models
{
    public class ArticleView
    {
        public string Heading { get; set; }

        public DateTime Updated { get; set; }

        public string Link { get; set; }

        public ArticleView(ArticleSource articleSource)
        {
            Heading = articleSource.Title;
            Updated = articleSource.UpdatedDate;
            Link = articleSource.ShortUrl;
        }
    }
}
