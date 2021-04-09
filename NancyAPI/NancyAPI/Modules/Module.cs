using Nancy;
using NancyAPI.Models;
using NancyAPI.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NancyAPI.Modules
{
    public class Module : NancyModule
    {
        private const string DEFAULT_ERROR_MESSAGE = "Something went wrong";
        private const string WELCOME_MESSAGE = "Hello!";
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private Regex Regex = new Regex("(http[s]?:[/])[^/]", RegexOptions.IgnoreCase, new TimeSpan(0, 0, 0, 1));

        private readonly ArticlesService m_ArticlesService;

        public Module(ArticlesService articlesService)
        {
            m_ArticlesService = articlesService;

            Get("/", _ => ExecuteAction(() => CheckConnect()));
            Get("/list/{section}/first", args => ExecuteAction(() => GetFirstArticle(args.section)));
            Get("/list/{section}/{updatedDate}", args => ExecuteAction(() => GetArticlesByDate(args.section, args.updatedDate)));
            Get("/list/{section}", args => ExecuteAction(() => GetArticles(args.section)));
            Get("/article/{shortUrl*}", args => ExecuteAction(() => GetArticlesByShortUrl(args.shortUrl)));
            Get("/group/{section}", args => ExecuteAction(() => GetArticlesGroupsByDate(args.section)));
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

        private string GetFirstArticle(string section)
        {
            var sourceArticles = m_ArticlesService.GetData(section);
            if (!sourceArticles.Any())
                throw new NancyAPIExeption("There are no articles for this section");

            var article = new ArticleView(sourceArticles.First());
            return JsonConvert.SerializeObject(article);
        }

        private string GetArticles(string section)
        {
            var sourceArticles = m_ArticlesService.GetData(section);
            var articles = sourceArticles.Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
            return JsonConvert.SerializeObject(articles);
        }

        private string GetArticlesByDate(string section, string updatedDateStr)
        {
            if (!DateTime.TryParse(updatedDateStr, out var updatedDate))
                throw new NancyAPIExeption($"The date has an incorrect format. Expected format: {DATE_FORMAT}");

            var sourceArticles = m_ArticlesService.GetData(section);
            var articles = sourceArticles.Where(sourceArticle => sourceArticle.UpdatedDate.Date == updatedDate.Date)
                .Select(sourceArticle => new ArticleView(sourceArticle)).ToList();
            return JsonConvert.SerializeObject(articles);
        }

        private string GetArticlesByShortUrl(string shortUrl)
        {
            shortUrl = GetCorrectedUrl(shortUrl);
            var sourceArticles = m_ArticlesService.GetData();
            var article = sourceArticles.FirstOrDefault(a => a.ShortUrl == shortUrl);
            if (article == null)
                throw new NancyAPIExeption($"The article with the url {shortUrl} was not found");
            
            return JsonConvert.SerializeObject(new ArticleView(article));
        }

        private string GetCorrectedUrl(string url)
        {
            if (Regex.IsMatch(url))
            {
                var invalidPart = Regex.Match(url).Groups[1].Value;
                url = url.Replace(invalidPart, invalidPart + @"/");
            }
            return url;
        }

        private string GetArticlesGroupsByDate(string section)
        {
            var sourceArticles = m_ArticlesService.GetData(section);
            var articleGroups = sourceArticles.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => new ArticleGroupByDateView(group.Key.ToString(DATE_FORMAT), group.Count())).ToList();
            return JsonConvert.SerializeObject(articleGroups);
        }

        private string ExecuteAction(Func<string> func)
        {
            try
            {
                return func();
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
