using Nancy;
using NancyAPI.Core;
using NancyAPI.Core.Services;
using NancyAPI.Core.Utils;
using NancyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyAPI.Modules
{
    public class Module : NancyModule
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string SHORT_URL_FORMAT = "XXXXXXX";
        private const string DEFAULT_ERROR_MESSAGE = "Something went wrong";
        private const string WELCOME_MESSAGE = "Hello!";

        private readonly ArticlesService m_ArticlesService;

        public Module(ArticlesService articlesService)
        {
            m_ArticlesService = articlesService;

            Get("/", async _ => 
                await ExecuteAction(async () => await CheckConnect()));

            Get("/list/{section}/first", async args => 
                await ExecuteAction(async () => await GetFirstArticle(args.section)));

            Get("/list/{section}/{updatedDate}", async args => 
                await ExecuteAction(async () => await GetArticlesByDate(args.section, args.updatedDate)));

            Get("/list/{section}", async args => 
                await ExecuteAction(async () => await GetArticles(args.section)));

            Get("/article/{shortUrl}", async args => 
                await ExecuteAction(async () => await GetArticlesByShortUrl(args.shortUrl)));

            Get("/group/{section}", async args => 
                await ExecuteAction(async () => await GetArticlesGroupsByDate(args.section)));
        }

        private async Task<ArticleView> GetFirstArticle(string section)
        {
            return new ArticleView(await m_ArticlesService.GetFirstArticle(section));
        }

        private async Task<List<ArticleView>> GetArticlesByDate(string section, string updatedDateStr)
        {
            return (await m_ArticlesService.GetArticlesByDate(section, updatedDateStr, DATE_FORMAT))
                .Select(articleSource => new ArticleView(articleSource)).ToList();
        }

        private async Task<List<ArticleView>> GetArticles(string section)
        {
            return (await m_ArticlesService.GetArticles(section))
                .Select(articleSource => new ArticleView(articleSource)).ToList();
        }

        private async Task<ArticleView> GetArticlesByShortUrl(string shortUrl)
        {
            return new ArticleView(await m_ArticlesService.GetArticlesByShortUrl(shortUrl, SHORT_URL_FORMAT));
        }

        private async Task<List<ArticleGroupByDateView>> GetArticlesGroupsByDate(string section)
        {
            return (await m_ArticlesService.GetArticlesGroupsByDate(section, DATE_FORMAT))
                .Select(group => new ArticleGroupByDateView(group.Item1, group.Item2)).ToList();
        }

        private async Task<string> CheckConnect()
        {
            var isConnected = await m_ArticlesService.IsConnected();
            if (isConnected.Item1)
            {
                return WELCOME_MESSAGE;
            }
            else
            {
                return isConnected.Item2;
            }
        }

        private async Task<string> ExecuteAction(Func<Task<object>> func)
        {
            try
            {
                return Converter.ToJson(await func());
            }
            catch (NancyAPICoreExeption ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return DEFAULT_ERROR_MESSAGE;
            }
        }
    }
}
