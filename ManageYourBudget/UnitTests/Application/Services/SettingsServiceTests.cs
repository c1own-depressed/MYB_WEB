using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Application.Services
{
    public class SettingsServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SettingsService>> _loggerMock;
        private readonly SettingsService _settingsService;

        public SettingsServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SettingsService>>();
            _settingsService = new SettingsService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserSettingsAsync_UserExists_ReturnsUserSettings()
        {
            // Arrange
            var userId = "user1";
            var user = new User
            {
                Id = userId,
                Currency = "EUR",
                IsLightTheme = false,
                Language = "French"
            };

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _settingsService.GetUserSettingsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("EUR", result.Currency);
            Assert.False(result.IsLightTheme);
            Assert.Equal("French", result.Language);
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserSettingsAsync_UserNotFound_ReturnsDefaultSettings()
        {
            // Arrange
            var userId = "user1";

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _settingsService.GetUserSettingsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Id);
            Assert.Equal("USD", result.Currency);
            Assert.True(result.IsLightTheme);
            Assert.Equal("English", result.Language);
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task SaveSettings_UserExists_UpdatesUserSettings()
        {
            // Arrange
            var userId = "user1";
            var user = new User
            {
                Id = userId,
                Currency = "USD",
                IsLightTheme = true,
                Language = "English"
            };
            var settingsDTO = new SettingsDTO
            {
                Id = userId,
                Currency = "EUR",
                IsLightTheme = false,
                Language = "French"
            };

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            await _settingsService.SaveSettings(settingsDTO);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Users.Update(It.Is<User>(u =>
                u.Id == userId &&
                u.Currency == "EUR" &&
                u.IsLightTheme == false &&
                u.Language == "French"
            )), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task SaveSettings_UserNotFound_DoesNotUpdateUserSettings()
        {
            // Arrange
            var userId = "user1";
            var settingsDTO = new SettingsDTO
            {
                Id = userId,
                Currency = "EUR",
                IsLightTheme = false,
                Language = "French"
            };

            _unitOfWorkMock.Setup(uow => uow.Users.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            await _settingsService.SaveSettings(settingsDTO);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.Users.GetByIdAsync(userId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Users.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Never);
        }
    }
}
