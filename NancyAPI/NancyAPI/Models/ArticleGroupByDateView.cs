using System;

namespace NancyAPI.Models
{
    public class ArticleGroupByDateView : IComparable<ArticleGroupByDateView>
    {
        public string Date { get; set; }

        public int Total { get; set; }

        public ArticleGroupByDateView(string date, int total) => (Date, Total) = (date, total);

        public int CompareTo(ArticleGroupByDateView other)
        {
            if (other == null ||
                Date != other.Date ||
                Total != other.Total) return -1;
            else
                return 0;
        }
    }
}
