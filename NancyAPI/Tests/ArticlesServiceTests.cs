using Moq;
using NancyAPI.Core.Models;
using NancyAPI.Core.Services;
using NancyAPI.Core.Utils;
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
        private const string SHORT_URL_FORMAT = "XXXXXXX";
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
        
        private readonly Mock<IArticlesSourceService> m_MockArticlesSourceService;

        public ArticlesServiceTests()
        {
            m_MockArticlesSourceService = new Mock<IArticlesSourceService>();
        }

        [Fact]
        public async Task IsConnectedAsync_ReturnTrue_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(null)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);

            // Act
            var isConnected = await articlesService.IsConnectedAsync();

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(null), Times.Once);
            Assert.True(isConnected.Item1, isConnected.Item2);
        }

        [Fact]
        public async Task IsConnectedAsync_ReturnFalse_WhenErrorGetData()
        {
            // Arrange
            var errorMessage = "Server error";
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(null)).ThrowsAsync(new NancyAPICoreExeption(errorMessage));
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);

            // Act
            var isConnected = await articlesService.IsConnectedAsync();

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(null), Times.Once);
            Assert.False(isConnected.Item1);
            Assert.Equal(isConnected.Item2, errorMessage);
        }

        [Fact]
        public async Task GetFirstArticleAsync_ReturnArticle_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(SECTION)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);
            var expectedArticle = ARTICLE_SOURCES.First();

            // Act
            var article = await articlesService.GetFirstArticleAsync(SECTION);

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(SECTION), Times.Once);
            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public async Task GetArticlesAsync_ReturnListOfArticles_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(SECTION)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);
            var expectedArticles = ARTICLE_SOURCES;

            // Act
            var articles = await articlesService.GetArticlesAsync(SECTION);

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(SECTION), Times.Once);
            Assert.Equal(expectedArticles, articles);
        }

        [Fact]
        public async Task GetArticlesByDateAsync_ReturnFiltredByDateArticles_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(SECTION)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);
            var expectedArticleSource = ARTICLE_SOURCES.First();
            var date = expectedArticleSource.UpdatedDate;
            var expectedArticles = ARTICLE_SOURCES.Where(a => a.UpdatedDate == date).ToList();

            // Act
            var articles = await articlesService.GetArticlesByDateAsync(SECTION, date.ToString(DATE_FORMAT), DATE_FORMAT);

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(SECTION), Times.Once);
            Assert.Equal(expectedArticles, articles);
        }

        [Fact]
        public async Task GetArticlesByShortUrlAsync_ReturnFiltredByShortUrl_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(null)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);
            var expectedArticle = ARTICLE_SOURCES.Last();
            var shortUrl = expectedArticle.ShortUrl;

            // Act
            var article = await articlesService.GetArticlesByShortUrlAsync(shortUrl, SHORT_URL_FORMAT);

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(null), Times.Once);
            Assert.Equal(expectedArticle, article);
        }

        [Fact]
        public async Task GetArticlesGroupsByDateAsync_ReturnCountOfArticlesGroupedByDate_WhenGetData()
        {
            // Arrange
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(SECTION)).ReturnsAsync(ARTICLE_SOURCES.ToList());
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);
            var expectedArticleGroups = ARTICLE_SOURCES.GroupBy(sourceArticle => sourceArticle.UpdatedDate.Date)
                .Select(group => (group.Key.ToString(DATE_FORMAT), group.Count())).ToList();

            // Act
            var articleGroups = await articlesService.GetArticlesGroupsByDateAsync(SECTION, DATE_FORMAT);

            // Assert
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(SECTION), Times.Once);
            Assert.Equal(expectedArticleGroups, articleGroups);
        }

        [Fact]
        public void GetArticlesByInvalidSectionAsync_ReturnException_WhenErrorGetData()
        {
            // Arrange
            var errorMessage = "Invalid section";
            m_MockArticlesSourceService.Setup(service => service.GetDataAsync(SECTION)).ThrowsAsync(new NancyAPICoreExeption(errorMessage));
            var articlesService = new ArticlesService(m_MockArticlesSourceService.Object);

            // Act
            Action act = () => articlesService.GetArticlesAsync(SECTION).GetAwaiter().GetResult();

            // Assert
            var exception = Assert.Throws<NancyAPICoreExeption>(act);
            Assert.Equal(errorMessage, exception.Message);
            m_MockArticlesSourceService.Verify(service => service.GetDataAsync(SECTION), Times.Once);
        }
    }
}
