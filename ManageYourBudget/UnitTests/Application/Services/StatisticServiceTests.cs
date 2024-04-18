using Moq;
using Application.Services;
using Domain.Interfaces;
using Domain.Entities;
using Application.DTOs.IncomeDTOs;
using Application.DTOs.StatisticDTO;

namespace UnitTests.Application.Services
{
    public class StatisticServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly StatisticService _service;

        public StatisticServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new StatisticService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetTotalExpensesByDate_NoExpenses_ReturnsEmpty()
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
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTotalExpensesByDate_WithExpenses_ReturnsGroupedTotals()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            var from = new DateTime(2023, 01, 01);
            var to = new DateTime(2023, 12, 31);
            var categories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = Guid.NewGuid().ToString(), UserId = userId },
                new ExpenseCategory { Id = Guid.NewGuid().ToString(), UserId = userId }
            };

            var expenses = new List<Expense>
            {
                new Expense { Id = Guid.NewGuid().ToString(), CategoryId = Guid.NewGuid().ToString(), Amount = 100, Date = new DateTime(2023, 01, 15) },
                new Expense { Id = Guid.NewGuid().ToString(),  CategoryId = Guid.NewGuid().ToString(), Amount = 200, Date = new DateTime(2023, 01, 20) },
                new Expense { Id = Guid.NewGuid().ToString(),  CategoryId = Guid.NewGuid().ToString(), Amount = 150, Date = new DateTime(2023, 02, 10) }
            };

            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId))
                           .ReturnsAsync(categories);
            _mockUnitOfWork.Setup(u => u.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(Guid.NewGuid().ToString(), from, to))
                           .ReturnsAsync((string id, DateTime f, DateTime t) => expenses.Where(e => e.CategoryId == id).ToList());

            // Act
            var result = await _service.GetTotalExpensesByDate(from, to, userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(300, resultList.Find(r => r.Month == new DateTime(2023, 01, 01)).TotalAmount);
            Assert.Equal(150, resultList.Find(r => r.Month == new DateTime(2023, 02, 01)).TotalAmount);
        }
        
    }

}


