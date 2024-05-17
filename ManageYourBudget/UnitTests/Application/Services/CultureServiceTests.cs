using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Application.Services
{
    public class CultureServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CultureService _cultureService;

        public CultureServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cultureService = new CultureService(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetUserCultureAsync_UserExists_ReturnsUserCulture()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId, Language = "en-US" };

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _cultureService.GetUserCultureAsync(userId);

            // Assert
            Assert.Equal("en-US", result);
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserCultureAsync_UserDoesNotExist_ReturnsDefaultCulture()
        {
            // Arrange
            var userId = "123";

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _cultureService.GetUserCultureAsync(userId);

            // Assert
            Assert.Equal("en-US", result);
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserCultureAsync_UserExistsButCultureIsNull_ReturnsDefaultCulture()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId, Language = null };

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _cultureService.GetUserCultureAsync(userId);

            // Assert
            Assert.Equal("en-US", result);
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
        }
    }
}
