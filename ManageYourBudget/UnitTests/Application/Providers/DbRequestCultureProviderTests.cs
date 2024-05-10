using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace UnitTests.Application.Providers
{
    public class DbRequestCultureProviderTests
    {
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<ILogger<DbRequestCultureProvider>> _mockLogger;
        private readonly DbRequestCultureProvider _provider;

        public DbRequestCultureProviderTests()
        {
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockLogger = new Mock<ILogger<DbRequestCultureProvider>>();
            _provider = new DbRequestCultureProvider(_mockScopeFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DetermineProviderCultureResult_WithUserIdAndNoCulture_ReturnsNull()
        {
            // Arrange
            var userId = "user123";
            var claimsIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(claimsIdentity);

            var mockScope = new Mock<IServiceScope>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockCultureService = new Mock<ICultureService>();
            mockCultureService.Setup(x => x.GetUserCultureAsync(userId)).ReturnsAsync((string)null);

            mockServiceProvider.Setup(x => x.GetService(typeof(ICultureService))).Returns(mockCultureService.Object);
            mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
            _mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

            // Act
            var result = await _provider.DetermineProviderCultureResult(mockHttpContext);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DetermineProviderCultureResult_WithUserIdAndCulture_ReturnsCulture()
        {
            // Arrange
            var userId = "user123";
            var cultureName = "en-US";
            var claimsIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) });
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(claimsIdentity);

            var mockScope = new Mock<IServiceScope>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockCultureService = new Mock<ICultureService>();
            mockCultureService.Setup(x => x.GetUserCultureAsync(userId)).ReturnsAsync(cultureName);

            mockServiceProvider.Setup(x => x.GetService(typeof(ICultureService))).Returns(mockCultureService.Object);
            mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
            _mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

            // Act
            var result = await _provider.DetermineProviderCultureResult(mockHttpContext);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cultureName, result.Cultures[0]);
            Assert.Equal(cultureName, result.UICultures[0]);
        }
    }
}
