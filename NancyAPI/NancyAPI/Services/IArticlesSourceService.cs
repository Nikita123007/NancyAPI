using NancyAPI.Models;
using System.Collections.Generic;

namespace NancyAPI.Services
{
    public interface IArticlesSourceService
    {
        List<ArticleSource> GetData(string section = null);
    }
}
