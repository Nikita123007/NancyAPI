using Moq;
using NancyAPI.Models;
using NancyAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ArticlesServiceTests
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string SECTION = "home";

        private IReadOnlyList<ArticleSource> ARTICLE_SOURCES = new List<ArticleSource>()
        {
            new ArticleSource() { Title = "Title1", Section = "Section1", ShortUrl = "ShtUrl1", UpdatedDate = new DateTime(1, 1, 1) },
            new ArticleSource() { Title = "Title2", Section = "Section2", ShortUrl = "ShtUrl2", UpdatedDate = new DateTime(1, 1, 1) },
            new ArticleSource() { Title = "Title3", Section = "Section3", ShortUrl = "ShtUrl3", UpdatedDate = new DateTime(1, 1, 1) },
            new ArticleSource() { Title = "Title4", Section = "Section4", ShortUrl = "ShtUrl4", UpdatedDate = new DateTime(1, 1, 2) },
            new ArticleSource() { Title = "Title5", Section = "Section5", ShortUrl = "ShtUrl5", UpdatedDate = new DateTime(1, 1, 2) },
            new ArticleSource() { Title = "Title6", Section = "Section6", ShortUrl = "ShtUrl6", UpdatedDate = new DateTime(1, 1, 2) },
            new ArticleSource() { Title = "Title7", Section = "Section7", ShortUrl = "ShtUrl7", UpdatedDate = new DateTime(1, 1, 3) },
            new ArticleSource() { Title = "Title8", Section = "Section8", ShortUrl = "ShtUrl8", UpdatedDate = new DateTime(1, 1, 3) },
            new ArticleSource() { Title = "Title9", Section = "Section9", ShortUrl = "ShtUrl9", UpdatedDate = new DateTime(1, 1, 3) },
        }.AsReadOnly();

        private readonly ArticlesService m_ArticlesService;

        public ArticlesServiceTests()
        {
            var mockArticlesSourceService = new Mock<IArticlesSourceService>();
            mockArticlesSourceService.Setup(service => service.GetData(SECTION)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            mockArticlesSourceService.Setup(service => service.GetData(null)).ReturnsAsync(ARTICLE_SOURCES.ToList());

            m_ArticlesService = new ArticlesService(mockArticlesSourceService.Object);
        }

        [Fact]
        public async Task IsConnected()
        {
            var isConnected = await m_ArticlesService.IsConnected();
            Assert.True(isConnected.Item1, isConnected.Item2);
        }

        [Fact]
        public async Task GetFirstArticle()
        {
            var article = await m_ArticlesService.GetFirstArticle(SECTION);

            var expectedArticle = new ArticleView(ARTICLE_SOURCES.First());

            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public async Task GetArticles()
        {
            var articles = await m_ArticlesService.GetArticles(SECTION);

            var expectedArticles = ARTICLE_SOURCES.Select(a => new ArticleView(a)).ToList();

            Assert.Equal(expectedArticles, articles);
        }

        [Fact]
        public async Task GetArticlesByDate()
        {
            var expectedArticleSource = ARTICLE_SOURCES.First();
            var date = expectedArticleSource.UpdatedDate;

            var articles = await m_ArticlesService.GetArticlesByDate(SECTION, date.ToString(DATE_FORMAT));

            var expectedArticles = ARTICLE_SOURCES.Where(a => a.UpdatedDate == date)
                .Select(a => new ArticleView(a)).ToList();

            Assert.Equal(expectedArticles, articles);
        }

        [Fact]
        public async Task GetArticlesByShortUrl()
        {
            var expectedArticleSource = ARTICLE_SOURCES.Last();
            var shortUrl = expectedArticleSource.ShortUrl;

            var article = await m_ArticlesService.GetArticlesByShortUrl(shortUrl);

            var expectedArticle = new ArticleView(expectedArticleSource);

            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public async Task GetArticlesGroupsByDate()
        {
            var articleGroups = await m_ArticlesService.GetArticlesGroupsByDate(SECTION);

            var expectedArticleGroups = ARTICLE_SOURCES.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => new ArticleGroupByDateView(group.Key.ToString(DATE_FORMAT), group.Count())).ToList();

            Assert.Equal(expectedArticleGroups, articleGroups);
        }
    }
}
