using System;

namespace NancyAPI.Models
{
    public class ArticleView : IComparable<ArticleView>
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

        public int CompareTo(ArticleView other)
        {
            if (other == null ||
                Heading != other.Heading ||
                Updated != other.Updated ||
                Link != other.Link) return -1;
            else
                return 0;
        }
    }
}
