using NancyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyAPI.Services
{
    public class ArticlesService
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string SHORT_URL_FORMAT = "XXXXXXX";

        private readonly IArticlesSourceService m_ArticlesSourceService;

        public ArticlesService(IArticlesSourceService articlesSourceService)
        {
            m_ArticlesSourceService = articlesSourceService;
        }
        
        public async Task<ArticleView> GetFirstArticle(string section)
        {
            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            if (!sourceArticles.Any())
                throw new NancyAPIExeption("There are no articles for this section");

            return new ArticleView(sourceArticles.First());
        }

        public async Task<List<ArticleView>> GetArticles(string section)
        {
            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            return sourceArticles.Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
        }

        public async Task<List<ArticleView>> GetArticlesByDate(string section, string updatedDateStr)
        {
            if (!DateTime.TryParse(updatedDateStr, out var updatedDate))
                throw new NancyAPIExeption($"The date has an incorrect format. Expected format: {DATE_FORMAT}");

            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            return sourceArticles.Where(sourceArticle => sourceArticle.UpdatedDate.Date == updatedDate.Date)
                .Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
        }

        public async Task<ArticleView> GetArticlesByShortUrl(string shortUrl)
        {
            if (shortUrl.Length != SHORT_URL_FORMAT.Length)
                throw new NancyAPIExeption($"The date has an incorrect format. Expected format: {SHORT_URL_FORMAT}");

            var sourceArticles = await m_ArticlesSourceService.GetData();
            var article = sourceArticles.FirstOrDefault(a => a.ShortUrl.EndsWith(shortUrl));
            if (article == null)
                throw new NancyAPIExeption($"The article with the url {shortUrl} was not found");

            return new ArticleView(article);
        }

        public async Task<List<ArticleGroupByDateView>> GetArticlesGroupsByDate(string section)
        {
            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            return sourceArticles.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => new ArticleGroupByDateView(group.Key.ToString(DATE_FORMAT), group.Count())).ToList();
        }

        public async Task<(bool, string)> IsConnected()
        {
            try
            {
                await m_ArticlesSourceService.GetData();
                return (true, null);
            }
            catch (NancyAPIExeption ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
