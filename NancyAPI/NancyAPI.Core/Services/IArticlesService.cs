using NancyAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyAPI.Core.Services
{
    public interface IArticlesService
    {

        Task<ArticleSource> GetFirstArticleAsync(string section);

        Task<List<ArticleSource>> GetArticlesAsync(string section);

        Task<IEnumerable<ArticleSource>> GetArticlesByDateAsync(string section, string updatedDateStr, string dateFormat);

        Task<ArticleSource> GetArticlesByShortUrlAsync(string shortUrl, string shortUrlFormat);

        Task<IEnumerable<(string, int)>> GetArticlesGroupsByDateAsync(string section, string dateFormat);

        Task<(bool, string)> IsConnectedAsync();
    }
}
