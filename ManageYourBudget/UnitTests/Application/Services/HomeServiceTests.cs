using Application.DTOs;
using Application.DTOs.ExpenseDTOs;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.SavingsDTOs;
using Application.Interfaces;
using Application.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Application.Services
{
    public class HomeServiceTests
    {
        private readonly Mock<IExpenseCategoryService> _expenseCategoryServiceMock;
        private readonly Mock<IIncomeService> _incomeServiceMock;
        private readonly Mock<ISavingsService> _savingsServiceMock;
        private readonly HomeService _homeService;

        public HomeServiceTests()
        {
            _expenseCategoryServiceMock = new Mock<IExpenseCategoryService>();
            _incomeServiceMock = new Mock<IIncomeService>();
            _savingsServiceMock = new Mock<ISavingsService>();
            _homeService = new HomeService(_expenseCategoryServiceMock.Object, _incomeServiceMock.Object, _savingsServiceMock.Object);
        }

        [Fact]
        public async Task GetHomeDataAsync_ValidUserId_ReturnsHomeDTO()
        {
            // Arrange
            var userId = "user1";
            var categories = new List<ExpenseCategoryDTO>
            {
                new ExpenseCategoryDTO { Id = "1", Name = "Food", PlannedBudget = 500, RemainingBudget = 300 },
                new ExpenseCategoryDTO { Id = "2", Name = "Utilities", PlannedBudget = 200, RemainingBudget = 100 }
            };
            var incomes = new List<IncomeDTO>
            {
                new IncomeDTO { Id = "1", IncomeName = "Job", Amount = 1000, CurrencyEmblem = "$", IsRegular = true, Date = DateTime.UtcNow },
                new IncomeDTO { Id = "2", IncomeName = "Freelance", Amount = 200, CurrencyEmblem = "$", IsRegular = false, Date = DateTime.UtcNow }
            };
            var savings = new List<SavingsDTO>
            {
                new SavingsDTO { Id = "1", SavingsName = "Vacation Fund", Amount = 500, Date = DateTime.UtcNow, CurrencyEmblem = "$", UserId = userId },
                new SavingsDTO { Id = "2", SavingsName = "Emergency Fund", Amount = 300, Date = DateTime.UtcNow, CurrencyEmblem = "$", UserId = userId }
            };

            _expenseCategoryServiceMock.Setup(service => service.GetExpenseCategoriesByUserIdAsync(userId))
                .ReturnsAsync(categories);
            _incomeServiceMock.Setup(service => service.GetIncomesByUserIdAsync(userId))
                .ReturnsAsync(incomes);
            _savingsServiceMock.Setup(service => service.GetSavingsByUserIdAsync(userId))
                .ReturnsAsync(savings);

            // Act
            var result = await _homeService.GetHomeDataAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categories, result.Categories);
            Assert.Equal(incomes, result.Incomes);
            Assert.Equal(savings, result.Savings);
        }

        [Fact]
        public async Task GetHomeDataAsync_UserHasNoData_ReturnsEmptyHomeDTO()
        {
            // Arrange
            var userId = "user1";
            var emptyCategories = new List<ExpenseCategoryDTO>();
            var emptyIncomes = new List<IncomeDTO>();
            var emptySavings = new List<SavingsDTO>();

            _expenseCategoryServiceMock.Setup(service => service.GetExpenseCategoriesByUserIdAsync(userId))
                .ReturnsAsync(emptyCategories);
            _incomeServiceMock.Setup(service => service.GetIncomesByUserIdAsync(userId))
                .ReturnsAsync(emptyIncomes);
            _savingsServiceMock.Setup(service => service.GetSavingsByUserIdAsync(userId))
                .ReturnsAsync(emptySavings);

            // Act
            var result = await _homeService.GetHomeDataAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Categories);
            Assert.Empty(result.Incomes);
            Assert.Empty(result.Savings);
        }
    }
}
