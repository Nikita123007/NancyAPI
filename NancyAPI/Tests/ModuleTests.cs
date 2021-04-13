using Moq;
using Nancy;
using Nancy.Testing;
using NancyAPI.Core.Models;
using NancyAPI.Core.Services;
using NancyAPI.Core.Utils;
using NancyAPI.Models;
using NancyAPI.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ModuleTests
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string SHORT_URL_FORMAT = "XXXXXXX";
        private const string SECTION = "home";
        private const string DEFAULT_ERROR_MESSAGE = "Something went wrong";

        private const string KEY = "k0XA0k0jJGAVuv8Jr5wAIcKDGPuznmRJ";
        private const string HOMESECTION = "home";
        private const string URLTEMPLATE = "https://api.nytimes.com/svc/topstories/v2/{0}.json?api-key={1}";
        
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

        private readonly Browser m_Browser;
        private readonly Mock<IArticlesService> m_MockArticlesService;

        public ModuleTests()
        {
            m_MockArticlesService = new Mock<IArticlesService>();
            m_Browser = new Browser(with => with.Module(new Module(m_MockArticlesService.Object)));
        }

        [Fact]
        public async Task CheckConnectedAsync_ReturnWelcomeMessage_WhenReturnTrue()
        {
            // Arrange
            m_MockArticlesService.Setup(service => service.IsConnectedAsync()).ReturnsAsync((true, null));
            var welcomeMessage = "Hello!";

            // Act
            var result = await m_Browser.Get("/", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.IsConnectedAsync(), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(Converter.ToJson(welcomeMessage), result.Body.AsString());
        }

        [Fact]
        public async Task CheckConnectedAsync_ReturnInvalidApiKeyMessage_WhenReturnFalse()
        {
            // Arrange
            var invalidKeyMessage = "Invalid ApiKey";
            m_MockArticlesService.Setup(service => service.IsConnectedAsync()).ReturnsAsync((false, invalidKeyMessage));

            // Act
            var result = await m_Browser.Get("/", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.IsConnectedAsync(), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(Converter.ToJson(invalidKeyMessage), result.Body.AsString());
        }

        [Fact]
        public async Task CheckConnectedAsync_ReturnInvalidSectionMessage_WhenReturnFalse()
        {
            // Arrange
            var invalidSectionMessage = "Section not found: invalid_section";
            m_MockArticlesService.Setup(service => service.IsConnectedAsync()).ReturnsAsync((false, invalidSectionMessage));

            // Act
            var result = await m_Browser.Get("/", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.IsConnectedAsync(), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(Converter.ToJson(invalidSectionMessage), result.Body.AsString());
        }

        [Fact]
        public async Task GetFirstArticleAsync_ReturnArticle_WhenGetFirstArticleSource()
        {
            // Arrange
            var articleSource = ARTICLE_SOURCES.First();
            m_MockArticlesService.Setup(service => service.GetFirstArticleAsync(SECTION)).ReturnsAsync(articleSource);
            var expectArticle = Converter.ToJson(new ArticleView(articleSource));

            // Act
            var result = await m_Browser.Get($"/list/{SECTION}/first", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.GetFirstArticleAsync(SECTION), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectArticle, result.Body.AsString());
        }

        [Fact]
        public async Task GetArticlesByDateAsync_ReturnArticles_WhenGetArticlesSourceByDate()
        {
            // Arrange
            var updatedDate = ARTICLE_SOURCES.First().UpdatedDate;
            var updatedDateStr = updatedDate.ToString(DATE_FORMAT);
            var articlesSource = ARTICLE_SOURCES.Where(articleSource => articleSource.UpdatedDate == updatedDate).ToList();
            m_MockArticlesService.Setup(service => service.GetArticlesByDateAsync(SECTION, updatedDateStr, DATE_FORMAT)).ReturnsAsync(articlesSource);
            var expectArticles = Converter.ToJson(articlesSource.Select(articleSource => new ArticleView(articleSource)).ToList());

            // Act
            var result = await m_Browser.Get($"/list/{SECTION}/{updatedDateStr}", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.GetArticlesByDateAsync(SECTION, updatedDateStr, DATE_FORMAT), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectArticles, result.Body.AsString());
        }

        [Fact]
        public async Task GetArticlesAsync_ReturnArticles_WhenGetArticlesSource()
        {
            // Arrange
            var articlesSource = ARTICLE_SOURCES.ToList();
            m_MockArticlesService.Setup(service => service.GetArticlesAsync(SECTION)).ReturnsAsync(articlesSource);
            var expectArticles = Converter.ToJson(articlesSource.Select(articleSource => new ArticleView(articleSource)).ToList());

            // Act
            var result = await m_Browser.Get($"/list/{SECTION}", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.GetArticlesAsync(SECTION), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectArticles, result.Body.AsString());
        }

        [Fact]
        public async Task GetArticlesByShortUrlAsync_ReturnArticles_WhenGetArticlesSourceByShortUrl()
        {
            // Arrange
            var articleSource = ARTICLE_SOURCES.First();
            var shortUrl = articleSource.ShortUrl;
            m_MockArticlesService.Setup(service => service.GetArticlesByShortUrlAsync(shortUrl, SHORT_URL_FORMAT)).ReturnsAsync(articleSource);
            var expectArticle = Converter.ToJson(new ArticleView(articleSource));

            // Act
            var result = await m_Browser.Get($"/article/{shortUrl}", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.GetArticlesByShortUrlAsync(shortUrl, SHORT_URL_FORMAT), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectArticle, result.Body.AsString());
        }

        [Fact]
        public async Task GetArticlesGroupsByDateAsync_ReturnArticles_WhenGetArticlesSourceGroupsByDate()
        {
            // Arrange
            var articleGroups = ARTICLE_SOURCES.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => (group.Key.ToString(DATE_FORMAT), group.Count())).ToList();
            m_MockArticlesService.Setup(service => service.GetArticlesGroupsByDateAsync(SECTION, DATE_FORMAT)).ReturnsAsync(articleGroups);
            var expectArticleGroups = Converter.ToJson(articleGroups.Select(group => new ArticleGroupByDateView(group.Item1, group.Item2)).ToList());

            // Act
            var result = await m_Browser.Get($"/group/{SECTION}", with => {
                with.HttpRequest();
            });

            // Assert
            m_MockArticlesService.Verify(service => service.GetArticlesGroupsByDateAsync(SECTION, DATE_FORMAT), Times.Once);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectArticleGroups, result.Body.AsString());
        }
    }
}
