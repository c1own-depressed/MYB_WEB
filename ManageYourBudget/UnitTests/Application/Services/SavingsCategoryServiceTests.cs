using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using FluentAssertions;
using Application.DTOs.SavingsDTOs;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.Services
{
    public class SavingsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SavingsService _service;
        private readonly ILogger<SavingsService> _logger;

        public SavingsServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _logger = new Mock<ILogger<SavingsService>>().Object;
            _service = new SavingsService(_mockUnitOfWork.Object, _logger);
        }

        [Fact]
        public async Task AddSavingsAsync_WithNegativeAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Negative Savings", Amount = -100 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto, userId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Savings amount must be greater than 0.");
        }

        [Fact]
        public async Task AddSavingsAsync_WithZeroAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Zero Savings", Amount = 0 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto, userId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Savings amount must be greater than 0.");
        }

        [Fact]
        public async Task AddSavingsAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Valid Savings", Amount = 500 };

            _mockUnitOfWork.Setup(u => u.Savings.AddAsync(It.IsAny<Savings>()))
                           .Returns(Task.CompletedTask); // Simulate saving successfully
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto, userId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveSavingsAsync_ExistingSavings_RemovesSavings()
        {
            // Arrange
            string savingsId = Guid.NewGuid().ToString();
            var savingsToRemove = new Savings { Id = savingsId, UserId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync(savingsToRemove);

            // Act
            var serviceResult = await _service.RemoveSavingsAsync(savingsId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();

            _mockUnitOfWork.Verify(u => u.Savings.Delete(savingsToRemove), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveSavingsAsync_NonExistingSavings_ReturnsError()
        {
            // Arrange
            string savingsId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync((Savings)null);

            // Act
            var serviceResult = await _service.RemoveSavingsAsync(savingsId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Savings not found.");

            _mockUnitOfWork.Verify(u => u.Savings.Delete(It.IsAny<Savings>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task EditSavingsAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            string savingsId = Guid.NewGuid().ToString();
            var dto = new EditSavingsDTO { Id = savingsId, SavingsName = "Edited Savings", Amount = 1000 };
            var savings = new Savings { Id = savingsId, SavingsName = "Original Savings", Amount = 500, UserId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync(savings);

            // Act
            var serviceResult = await _service.EditSavingsAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();
            savings.SavingsName.Should().Be(dto.SavingsName);
            savings.Amount.Should().Be(dto.Amount);

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task EditSavingsAsync_NonExistingSavings_ReturnsError()
        {
            // Arrange
            string savingsId = Guid.NewGuid().ToString();
            var dto = new EditSavingsDTO { Id = savingsId, SavingsName = "Edited Savings", Amount = 1000 };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync((Savings)null);

            // Act
            var serviceResult = await _service.EditSavingsAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Savings not found.");

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}
