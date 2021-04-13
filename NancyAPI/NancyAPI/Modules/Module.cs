using Nancy;
using NancyAPI.Core.Services;
using NancyAPI.Core.Utils;
using NancyAPI.Models;
using NancyAPI.Utils;
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
                await ExecuteActionAsync(async () => await CheckConnectAsync()));

            Get("/list/{section}/first", async args => 
                await ExecuteActionAsync(async () => await GetFirstArticleAsync(args.section)));

            Get("/list/{section}/{updatedDate}", async args => 
                await ExecuteActionAsync(async () => await GetArticlesByDateAsync(args.section, args.updatedDate)));

            Get("/list/{section}", async args => 
                await ExecuteActionAsync(async () => await GetArticlesAsync(args.section)));

            Get("/article/{shortUrl}", async args => 
                await ExecuteActionAsync(async () => await GetArticlesByShortUrlAsync(args.shortUrl)));

            Get("/group/{section}", async args => 
                await ExecuteActionAsync(async () => await GetArticlesGroupsByDateAsync(args.section)));
        }

        private async Task<ArticleView> GetFirstArticleAsync(string section)
        {
            return new ArticleView(await m_ArticlesService.GetFirstArticleAsync(section));
        }

        private async Task<List<ArticleView>> GetArticlesByDateAsync(string section, string updatedDateStr)
        {
            return (await m_ArticlesService.GetArticlesByDateAsync(section, updatedDateStr, DATE_FORMAT))
                .Select(articleSource => new ArticleView(articleSource)).ToList();
        }

        private async Task<List<ArticleView>> GetArticlesAsync(string section)
        {
            return (await m_ArticlesService.GetArticlesAsync(section))
                .Select(articleSource => new ArticleView(articleSource)).ToList();
        }

        private async Task<ArticleView> GetArticlesByShortUrlAsync(string shortUrl)
        {
            return new ArticleView(await m_ArticlesService.GetArticlesByShortUrlAsync(shortUrl, SHORT_URL_FORMAT));
        }

        private async Task<List<ArticleGroupByDateView>> GetArticlesGroupsByDateAsync(string section)
        {
            return (await m_ArticlesService.GetArticlesGroupsByDateAsync(section, DATE_FORMAT))
                .Select(group => new ArticleGroupByDateView(group.Item1, group.Item2)).ToList();
        }

        private async Task<string> CheckConnectAsync()
        {
            var isConnected = await m_ArticlesService.IsConnectedAsync();
            if (isConnected.Item1)
            {
                return WELCOME_MESSAGE;
            }
            else
            {
                return isConnected.Item2;
            }
        }

        private async Task<string> ExecuteActionAsync(Func<Task<object>> func)
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
