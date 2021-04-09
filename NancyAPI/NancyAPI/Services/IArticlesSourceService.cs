using NancyAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NancyAPI.Services
{
    public interface IArticlesSourceService
    {
        Task<List<ArticleSource>> GetData(string section = null);
    }
}
