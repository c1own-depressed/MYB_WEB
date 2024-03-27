using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Application.Services
{
    public class SavingsServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SavingsService _service;

        public SavingsServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new SavingsService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddSavingsAsync_WithNegativeAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Negative Savings", Amount = -100 };

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Contains("Savings amount must be greater than 0.".Trim(), serviceResult.Errors.Select(e => e.Trim()));
        }

        [Fact]
        public async Task AddSavingsAsync_WithZeroAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Zero Savings", Amount = 0 };

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Contains("Savings amount must be greater than 0.", serviceResult.Errors);
        }

        [Fact]
        public async Task AddSavingsAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateSavingsDTO { SavingsName = "Valid Savings", Amount = 500 };

            _mockUnitOfWork.Setup(u => u.Savings.AddAsync(It.IsAny<Savings>()))
                           .Returns(Task.CompletedTask); // Simulate saving successfully

            // Act
            var serviceResult = await _service.AddSavingsAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            Assert.Empty(serviceResult.Errors);
        }

        [Fact]
        public async Task RemoveSavingsAsync_ExistingSavings_RemovesSavings()
        {
            // Arrange
            int savingsId = 1;
            var savingsToRemove = new Savings { Id = savingsId };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync(savingsToRemove);

            // Act
            var serviceResult = await _service.RemoveSavingsAsync(savingsId);

            // Assert
            Assert.True(serviceResult.Success);
            Assert.Empty(serviceResult.Errors);

            _mockUnitOfWork.Verify(u => u.Savings.Delete(savingsToRemove), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveSavingsAsync_NonExistingSavings_ReturnsError()
        {
            // Arrange
            int savingsId = 1;

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync((Savings)null);

            // Act
            var serviceResult = await _service.RemoveSavingsAsync(savingsId);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Contains("Savings not found.", serviceResult.Errors);

            _mockUnitOfWork.Verify(u => u.Savings.Delete(It.IsAny<Savings>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task EditSavingsAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            int savingsId = 1;
            var dto = new EditSavingsDTO { Id = savingsId, SavingsName = "Edited Savings", Amount = 1000 };
            var savings = new Savings { Id = savingsId, SavingsName = "Original Savings", Amount = 500 };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync(savings);

            // Act
            var serviceResult = await _service.EditSavingsAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            Assert.Empty(serviceResult.Errors);
            Assert.Equal(dto.SavingsName, savings.SavingsName);
            Assert.Equal(dto.Amount, savings.Amount);

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task EditSavingsAsync_NonExistingSavings_ReturnsError()
        {
            // Arrange
            int savingsId = 1;
            var dto = new EditSavingsDTO { Id = savingsId, SavingsName = "Edited Savings", Amount = 1000 };

            _mockUnitOfWork.Setup(u => u.Savings.GetByIdAsync(savingsId))
                           .ReturnsAsync((Savings)null);

            // Act
            var serviceResult = await _service.EditSavingsAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Contains("Savings not found.", serviceResult.Errors);

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}
