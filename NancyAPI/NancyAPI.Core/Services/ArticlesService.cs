using NancyAPI.Core.Models;
using NancyAPI.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyAPI.Core.Services
{
    public class ArticlesService
    {
        private readonly IArticlesSourceService m_ArticlesSourceService;

        public ArticlesService(IArticlesSourceService articlesSourceService)
        {
            m_ArticlesSourceService = articlesSourceService;
        }
        
        public async Task<ArticleSource> GetFirstArticle(string section)
        {
            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            if (!sourceArticles.Any())
            {
                throw new NancyAPICoreExeption("There are no articles for this section");
            }

            return sourceArticles.First();
        }

        public async Task<List<ArticleSource>> GetArticles(string section)
        {
            return await m_ArticlesSourceService.GetData(section);
        }

        public async Task<IEnumerable<ArticleSource>> GetArticlesByDate(string section, string updatedDateStr, string dateFormat)
        {
            if (!DateTime.TryParse(updatedDateStr, out var updatedDate))
            {
                throw new NancyAPICoreExeption($"The date has an incorrect format. Expected format: {dateFormat}");
            }

            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            return sourceArticles.Where(sourceArticle => sourceArticle.UpdatedDate.Date == updatedDate.Date);
        }

        public async Task<ArticleSource> GetArticlesByShortUrl(string shortUrl, string shortUrlFormat)
        {
            if (shortUrl.Length != shortUrlFormat.Length)
            {
                throw new NancyAPICoreExeption($"The date has an incorrect format. Expected format: {shortUrlFormat}");
            }

            var sourceArticles = await m_ArticlesSourceService.GetData();
            var article = sourceArticles.FirstOrDefault(a => a.ShortUrl.EndsWith(shortUrl));
            if (article == null)
            {
                throw new NancyAPICoreExeption($"The article with the url {shortUrl} was not found");
            }

            return article;
        }

        public async Task<IEnumerable<(string, int)>> GetArticlesGroupsByDate(string section, string dateFormat)
        {
            var sourceArticles = await m_ArticlesSourceService.GetData(section);
            return sourceArticles.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => (group.Key.ToString(dateFormat), group.Count()));
        }

        public async Task<(bool, string)> IsConnected()
        {
            try
            {
                await m_ArticlesSourceService.GetData();
                return (true, null);
            }
            catch (NancyAPICoreExeption ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
