using Nancy;
using NancyAPI.Services;
using Newtonsoft.Json;
using System;

namespace NancyAPI.Modules
{
    public class Module : NancyModule
    {
        private const string DEFAULT_ERROR_MESSAGE = "Something went wrong";
        private const string WELCOME_MESSAGE = "Hello!";

        private readonly ArticlesService m_ArticlesService;

        public Module(ArticlesService articlesService)
        {
            m_ArticlesService = articlesService;

            Get("/", _ => ExecuteAction(() => CheckConnect()));
            Get("/list/{section}/first", args => ExecuteAction(() => articlesService.GetFirstArticle(args.section)));
            Get("/list/{section}/{updatedDate}", args => ExecuteAction(() => articlesService.GetArticlesByDate(args.section, args.updatedDate)));
            Get("/list/{section}", args => ExecuteAction(() => articlesService.GetArticles(args.section)));
            Get("/article/{shortUrl}", args => ExecuteAction(() => articlesService.GetArticlesByShortUrl(args.shortUrl)));
            Get("/group/{section}", args => ExecuteAction(() => articlesService.GetArticlesGroupsByDate(args.section)));
        }

        private string CheckConnect()
        {
            if (m_ArticlesService.IsConnected(out var errorMessage))
            {
                return WELCOME_MESSAGE;
            }
            else
            {
                return errorMessage;
            }
        }

        private string ExecuteAction(Func<object> func)
        {
            try
            {
                return JsonConvert.SerializeObject(func());
            }
            catch (NancyAPIExeption ex)
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
