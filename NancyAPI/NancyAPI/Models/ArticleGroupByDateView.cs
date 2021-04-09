namespace NancyAPI.Models
{
    public class ArticleGroupByDateView
    {
        public string Date { get; set; }

        public int Total { get; set; }

        public ArticleGroupByDateView(string date, int total) => (Date, Total) = (date, total);
    }
}
