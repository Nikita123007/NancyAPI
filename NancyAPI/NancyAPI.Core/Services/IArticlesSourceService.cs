using NancyAPI.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyAPI.Core.Services
{
    public interface IArticlesSourceService
    {
        Task<List<ArticleSource>> GetDataAsync(string section = null);
    }
}
