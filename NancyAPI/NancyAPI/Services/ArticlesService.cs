using NancyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public ArticleView GetFirstArticle(string section)
        {
            var sourceArticles = m_ArticlesSourceService.GetData(section);
            if (!sourceArticles.Any())
                throw new NancyAPIExeption("There are no articles for this section");

            return new ArticleView(sourceArticles.First());
        }

        public List<ArticleView> GetArticles(string section)
        {
            var sourceArticles = m_ArticlesSourceService.GetData(section);
            return sourceArticles.Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
        }

        public List<ArticleView> GetArticlesByDate(string section, string updatedDateStr)
        {
            if (!DateTime.TryParse(updatedDateStr, out var updatedDate))
                throw new NancyAPIExeption($"The date has an incorrect format. Expected format: {DATE_FORMAT}");

            var sourceArticles = m_ArticlesSourceService.GetData(section);
            return sourceArticles.Where(sourceArticle => sourceArticle.UpdatedDate.Date == updatedDate.Date)
                .Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
        }

        public ArticleView GetArticlesByShortUrl(string shortUrl)
        {
            if (shortUrl.Length != SHORT_URL_FORMAT.Length)
                throw new NancyAPIExeption($"The date has an incorrect format. Expected format: {SHORT_URL_FORMAT}");

            var sourceArticles = m_ArticlesSourceService.GetData();
            var article = sourceArticles.FirstOrDefault(a => a.ShortUrl.EndsWith(shortUrl));
            if (article == null)
                throw new NancyAPIExeption($"The article with the url {shortUrl} was not found");

            return new ArticleView(article);
        }

        public List<ArticleGroupByDateView> GetArticlesGroupsByDate(string section)
        {
            var sourceArticles = m_ArticlesSourceService.GetData(section);
            return sourceArticles.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => new ArticleGroupByDateView(group.Key.ToString(DATE_FORMAT), group.Count())).ToList();
        }

        public bool IsConnected(out string errorMessage)
        {
            try
            {
                m_ArticlesSourceService.GetData();
                errorMessage = null;
                return true;
            }
            catch (NancyAPIExeption ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
