using Nancy;
using NancyAPI.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

            Get("/", async _ => 
                await ExecuteAction(async () => await CheckConnect()));

            Get("/list/{section}/first", async args => 
                await ExecuteAction(async () => await articlesService.GetFirstArticle(args.section)));

            Get("/list/{section}/{updatedDate}", async args => 
                await ExecuteAction(async () => await articlesService.GetArticlesByDate(args.section, args.updatedDate)));

            Get("/list/{section}", async args => 
                await ExecuteAction(async () => await articlesService.GetArticles(args.section)));

            Get("/article/{shortUrl}", async args => 
                await ExecuteAction(async () => await articlesService.GetArticlesByShortUrl(args.shortUrl)));

            Get("/group/{section}", async args => 
                await ExecuteAction(async () => await articlesService.GetArticlesGroupsByDate(args.section)));
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
                return JsonConvert.SerializeObject(await func());
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
