using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Application.Services
{
    public class IncomeCategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IncomeService _service;

        public IncomeCategoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new IncomeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddInomeAsync_WithNegativeAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "Negative Income", Amount = -100 };

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithZeroAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "Zero  Income", Amount = 0 };

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }


        [Fact]
        public async Task AddIncomeAsync_WithExtremelyHighAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "High Budget Category", Amount = double.MaxValue };

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be lower than 100000000.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithShortTitle_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "Pub", Amount = 450 };

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithLongTitle_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "Category New Category New Category New Category New Category New Category New Category New Category New Category New Category New", Amount = 450 };

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new IncomeDTO { IncomeName = "Valid Income", Amount = 5000 };
            _mockUnitOfWork.Setup(u => u.Incomes.AddAsync(It.IsAny<Income>()))
                           .Returns(Task.CompletedTask)
                           .Verifiable("AddAsync was not called with an ExpenseCategory object.");
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                           .ReturnsAsync(1)
                           .Verifiable("CompleteAsync was not called to save the changes.");

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Empty(errorMessages);

            _mockUnitOfWork.Verify();
        }

        [Fact]
        public async Task GetIncomesByUserIdAsync_WithValidUserId_ReturnsIncomeDTOs()
        {
            // Arrange
            int userId = 1;
            var expectedIncomes = new List<Income>
            {
                new Income { IncomeName = "Salary", Amount = 5000 },
                new Income { IncomeName = "Freelancing", Amount = 1500 }
            };

            _mockUnitOfWork.Setup(u => u.Incomes.GetIncomesByUserIdAsync(userId))
                .ReturnsAsync(expectedIncomes);

            // Act
            var result = await _service.GetIncomesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIncomes.Count, result.Count());

            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count; i++)
            {
                Assert.Equal(expectedIncomes[i].IncomeName, resultList[i].IncomeName);
                Assert.Equal(expectedIncomes[i].Amount, resultList[i].Amount);
            }
        }
    }
}
