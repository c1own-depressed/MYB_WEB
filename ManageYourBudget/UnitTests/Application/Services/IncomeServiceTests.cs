using Application.DTOs;
using Application.DTOs.IncomeDTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace UnitTests.Application.Services
{
    public class IncomeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly IncomeService _service;

        public IncomeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new IncomeService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddInomeAsync_WithNegativeAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "Negative Income", Amount = -100 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithZeroAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "Zero  Income", Amount = 0 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }


        [Fact]
        public async Task AddIncomeAsync_WithExtremelyHighAmount_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "High Budget Category", Amount = double.MaxValue };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be lower than 100000000.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithShortTitle_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "Pub", Amount = 450 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithLongTitle_ReturnsError()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "Category New Category New Category New Category New Category New Category New Category New Category New Category New Category New", Amount = 450 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddIncomeAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new IncomeDTO { Id = Guid.NewGuid().ToString(), IncomeName = "Valid Income", Amount = 5000 };
            _mockUnitOfWork.Setup(u => u.Incomes.AddAsync(It.IsAny<Income>()))
                           .Returns(Task.CompletedTask)
                           .Verifiable("AddAsync was not called with an ExpenseCategory object.");
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                           .ReturnsAsync(1)
                           .Verifiable("CompleteAsync was not called to save the changes.");
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddIncomeAsync(dto, userId);

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
            string userId = Guid.NewGuid().ToString(); ;
            var mockUser = new User 
            { 
                Id = userId,
                Email = "test@gmail.com",
                Currency = "usd",
                IsLightTheme = true,
                Language = "en",
            };

            var expectedIncomes = new List<Income>
            {
                new Income {  Id = Guid.NewGuid().ToString(), IncomeName = "Salary", Amount = 5000, UserId = Guid.NewGuid().ToString()  },
                new Income {  Id = Guid.NewGuid().ToString(), IncomeName = "Freelancing", Amount = 1500, UserId = Guid.NewGuid().ToString()  }
            };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(mockUser);

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

        // Remove Income
        [Fact]
        public async Task RemoveIncomeAsync_ExistingIncome_RemovesIncome()
        {
            // Arrange
            string incomeId = Guid.NewGuid().ToString();
            var incomeToRemove = new Income { Id = incomeId, UserId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Incomes.GetByIdAsync(incomeId))
                           .ReturnsAsync(incomeToRemove);

            // Act
            var serviceResult = await _service.RemoveIncomeAsync(incomeId);

            // Assert
            Assert.True(serviceResult.Success);
            Assert.Empty(serviceResult.Errors);

            _mockUnitOfWork.Verify(u => u.Incomes.Delete(incomeToRemove), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveIncomeAsync_NonExistingIncome_ReturnsError()
        {
            // Arrange
            string incomeId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.Incomes.GetByIdAsync(incomeId))
                           .ReturnsAsync((Income)null);

            // Act
            var serviceResult = await _service.RemoveIncomeAsync(incomeId);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Collection(serviceResult.Errors, error => Assert.Equal("Income not found.", error));

            _mockUnitOfWork.Verify(u => u.Incomes.Delete(It.IsAny<Income>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        // Edit Income
        [Fact]
        public async Task EditIncomeAsync_WithNegativeBudget_ReturnsError()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "Negative Amount Income", Amount = -100 };

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Amount must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task EditIncomeAsync_WithZeroBudget_ReturnsError()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "Zero Amount Income", Amount = 0 };

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Amount must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task EditIncomeAsync_WithExtremelyHighBudget_ReturnsError()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "High Amount Income", Amount = double.MaxValue };

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Amount must be lower than 100000000.", errorMessages);
        }

        [Fact]
        public async Task EditIncomeAsync_WithShortName_ReturnsError()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "Cate", Amount = 450 };

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Name length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task EditIncomeAsync_WithLongName_ReturnsError()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "Category New Category New Category New Category New Category New Category New Category New Category New Category New Category New", Amount = 450 };

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Name length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task EditIncomeAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var incomeId = Guid.NewGuid().ToString();
            var dto = new EditIncomeDTO { Id = incomeId, Name = "Valid Category", Amount = 5000 };
            _mockUnitOfWork.Setup(u => u.Incomes.GetByIdAsync(incomeId))
                           .ReturnsAsync(new Income { Id = incomeId, UserId = Guid.NewGuid().ToString() }); // Simulate getting an existing income
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                           .ReturnsAsync(1); // Simulate saving changes successfully

            // Act
            var serviceResult = await _service.EditIncomeAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Empty(errorMessages);
        }
    }
}
