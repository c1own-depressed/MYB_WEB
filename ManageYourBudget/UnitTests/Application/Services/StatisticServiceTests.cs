using Moq;
using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Application.DTOs.IncomeDTOs;

namespace UnitTests.Application.Services
{
    public class StatisticServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly StatisticService _service;
        private readonly Mock<IIncomeService> _mockIncomeService;
        private readonly Mock<IExpenseService> _mockExpenseService;

        public StatisticServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockIncomeService = new Mock<IIncomeService>();
            _mockExpenseService = new Mock<IExpenseService>();
            _service = new StatisticService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetTotalExpensesByDate_NoExpenses_ReturnsZeroesForAllMonths()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            var from = new DateTime(2023, 01, 01);
            var to = new DateTime(2023, 12, 31);
            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId))
                           .ReturnsAsync(new List<ExpenseCategory>());
            _mockUnitOfWork.Setup(u => u.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(Guid.NewGuid().ToString(), from, to))
                           .ReturnsAsync(new List<Expense>());

            // Act
            var result = await _service.GetTotalExpensesByDate(from, to, userId);

            // Assert
            var expectedMonthsCount = 12; // January to December
            Assert.Equal(expectedMonthsCount, result.Count());
            Assert.True(result.All(r => r.TotalAmount == 0));
        }

        //[Fact]
        //public async Task GetTotalExpensesByDate_WithExpenses_ReturnsCorrectTotalsIncludingZeroMonths()
        //{
        //    // Arrange
        //    string userId = Guid.NewGuid().ToString();
        //    var from = new DateTime(2023, 01, 01);
        //    var to = new DateTime(2023, 12, 31);
        //    var categories = new List<ExpenseCategory>
        //    {
        //        new ExpenseCategory { Id = Guid.NewGuid().ToString(), UserId = userId },
        //        new ExpenseCategory { Id = Guid.NewGuid().ToString(), UserId = userId }
        //    };

        //    var expenses = new List<Expense>
        //    {
        //        new Expense { Id = Guid.NewGuid().ToString(), CategoryId = Guid.NewGuid().ToString(), Amount = 100, Date = new DateTime(2023, 01, 15) },
        //        new Expense { Id = Guid.NewGuid().ToString(),  CategoryId = Guid.NewGuid().ToString(), Amount = 200, Date = new DateTime(2023, 01, 20) },
        //        new Expense { Id = Guid.NewGuid().ToString(),  CategoryId = Guid.NewGuid().ToString(), Amount = 150, Date = new DateTime(2023, 02, 10) }
        //    };

        //    _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId))
        //                   .ReturnsAsync(categories);
        //    _mockUnitOfWork.Setup(u => u.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(Guid.NewGuid().ToString(), from, to))
        //                   .ReturnsAsync((string id, DateTime f, DateTime t) => expenses.Where(e => e.CategoryId == id).ToList());

        //    // Act
        //    var result = await _service.GetTotalExpensesByDate(from, to, userId);

        //    // Assert
        //    var resultList = result.ToList();
        //    Assert.Equal(12, resultList.Count); // Should cover all months, January to December
        //    Assert.Equal(300, resultList.Find(r => r.Month == new DateTime(2023, 01, 01)).TotalAmount);
        //    Assert.Equal(150, resultList.Find(r => r.Month == new DateTime(2023, 02, 01)).TotalAmount);
        //    Assert.True(resultList.Where(r => r.Month > new DateTime(2023, 02, 01)).All(r => r.TotalAmount == 0)); // Ensuring other months are zero
        //}

        [Fact]
        public async Task GetIncomesByDate_NoIncomes_ReturnsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            DateTime startDate = new DateTime(2023, 01, 01);
            DateTime endDate = new DateTime(2023, 12, 31);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(new User { Currency = "usd" });
            _mockUnitOfWork.Setup(u => u.Incomes.GetIncomesByUserIdAsync(userId)).ReturnsAsync(new List<Income>());

            // Act
            var result = await _service.GetIncomesByDate(startDate, endDate, userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetIncomesByDate_RegularAndOneTimeIncomes_CorrectGroupingAndSumming()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            DateTime startDate = new DateTime(2023, 01, 01);
            DateTime endDate = new DateTime(2023, 03, 01);
            var incomes = new List<Income>
            {
                new Income { Id = Guid.NewGuid().ToString(), IncomeName = "Salary", Amount = 1000, Date = new DateTime(2023, 01, 10), IsRegular = true, UserId = userId },
                new Income { Id = Guid.NewGuid().ToString(), IncomeName = "Gift", Amount = 500, Date = new DateTime(2023, 01, 15), IsRegular = false, UserId = userId }
            };
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(new User { Currency = "usd" });
            _mockUnitOfWork.Setup(u => u.Incomes.GetIncomesByUserIdAsync(userId)).ReturnsAsync(incomes);

            // Act
            var result = await _service.GetIncomesByDate(startDate, endDate, userId);
            var resultList = new List<IncomeStatisticDTO>(result);

            // Assert
            Assert.Equal(3, resultList.Count); // Checking if all months are accounted for
            Assert.Equal(1500, resultList.Find(i => i.Month == new DateTime(2023, 01, 01)).TotalAmount);
            Assert.Equal(1000, resultList.Find(i => i.Month == new DateTime(2023, 02, 01)).TotalAmount);
            Assert.Equal(1000, resultList.Find(i => i.Month == new DateTime(2023, 03, 01)).TotalAmount); // Regular income continues
        }

        [Fact]
        public async Task CountSaved_NoIncomesOrExpenses_ReturnsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            DateTime from = new DateTime(2023, 01, 01);
            DateTime to = new DateTime(2023, 12, 31);

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                          .ReturnsAsync(new User() { Currency = "usd" });
            _mockUnitOfWork.Setup(u => u.Incomes.GetIncomesByUserIdAsync(userId))
                           .ReturnsAsync(new List<Income>());
            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId))
                          .ReturnsAsync(new List<ExpenseCategory>() { });
            _mockUnitOfWork.Setup(u => u.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(Guid.NewGuid().ToString(), from, to))
                           .ReturnsAsync(new List<Expense>());

            // Act
            var result = await _service.CountSaved(from, to, userId);

            // Assert
            Assert.Empty(result);
        }

        //[Fact]
        //public async Task CountSaved_IncomesGreaterThanExpenses_ReturnsPositiveSavings()
        //{
        //    // Arrange
        //    var userId = Guid.NewGuid().ToString();
        //    DateTime from = new DateTime(2023, 05, 01);
        //    DateTime to = new DateTime(2023, 05, 31);

        //    var incomes = new List<IncomeStatisticDTO>
        //    {
        //        new IncomeStatisticDTO { Month = new DateTime(2023, 05, 05), TotalAmount = 2000 }
        //    };
        //    var expenses = new List<TotalExpensesDTO>
        //    {
        //        new TotalExpensesDTO { Month = new DateTime(2023, 05, 05), TotalAmount = 1500 }
        //    };

        //    _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(new User { Currency = "usd" });
        //    _mockUnitOfWork.Setup(u => u.Incomes.GetIncomesByUserIdAsync(userId))
        //                   .ReturnsAsync(new List<Income>
        //                   {
        //               new Income { Id = Guid.NewGuid().ToString(), IncomeName = "Salary", Amount = 2000, Date = new DateTime(2023, 05, 05), IsRegular = true, UserId = userId }
        //                   });
        //    _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId))
        //                  .ReturnsAsync(new List<ExpenseCategory>() { });
        //    _mockUnitOfWork.Setup(u => u.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(Guid.NewGuid().ToString(), from, to))
        //                   .ReturnsAsync(new List<Expense>
        //                   {
        //               new Expense { Id = Guid.NewGuid().ToString(), Amount = 1500, Date = new DateTime(2023, 05, 05), CategoryId = Guid.NewGuid().ToString() }
        //                   });

        //    // Act
        //    var result = await _service.CountSaved(from, to, userId);

        //    // Assert
        //    var resultList = result.ToList();
        //    Assert.NotEmpty(resultList);
        //    Assert.Single(resultList);
        //    var savings = resultList.First();
        //    Assert.Equal(new DateTime(2023, 05, 01), savings.Month);
        //    Assert.Equal(500, savings.TotalAmount); // 2000 - 1500
        //}
    }
}

