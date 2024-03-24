﻿using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Moq;


namespace UnitTests.Application.Services
{
    public class ExpenseCategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IExpenseService> _mockExpenseService;
        private readonly ExpenseCategoryService _service;

        public ExpenseCategoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExpenseService = new Mock<IExpenseService>();
            _service = new ExpenseCategoryService(_mockUnitOfWork.Object, _mockExpenseService.Object);
        }

        [Fact]
        public async Task GetExpenseCategoriesByUserIdAsync_WithCategories_ReturnsCategoriesWithExpenses()
        {
            // Arrange
            int testUserId = 1;
            var testCategories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, CategoryName = "Test Category 1", Amount = 100, UserId = testUserId },
                new ExpenseCategory { Id = 2, CategoryName = "Test Category 2", Amount = 200, UserId = testUserId }
            };

            var testExpenses = new List<ExpenseDTO>
            {
                new ExpenseDTO { ExpenseName = "Expense 1", Amount = 50 },
                new ExpenseDTO { ExpenseName = "Expense 2", Amount = 75 }
            };

            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserId(testUserId))
               .Returns(testCategories);


            _mockExpenseService.Setup(s => s.GetExpensesByCategoryIdAsync(It.IsAny<int>()))
                               .ReturnsAsync(testExpenses);

            // Act
            var result = await _service.GetExpenseCategoriesByUserIdAsync(testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testCategories.Count, result.Count());

            // Further asserts to match specific properties if necessary
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count; i++)
            {
                Assert.Equal(testCategories[i].CategoryName, resultList[i].Name);
                Assert.Equal(testExpenses.Sum(e => e.Amount), resultList[i].Expenses.Sum(e => e.Amount));
            }
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithNegativeBudget_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Negative Budget Category", PlannedBudget = -100 };

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithZeroBudget_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Zero Budget Category", PlannedBudget = 0 };

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }


        [Fact]
        public async Task AddExpenseCategoryAsync_WithExtremelyHighBudget_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "High Budget Category", PlannedBudget = double.MaxValue };

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be lower than 100000000.", errorMessages);
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithShortTitle_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Cate", PlannedBudget = 450 };

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);
            
            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithLongTitle_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Category New Category New Category New Category New Category New Category New Category New Category New Category New Category New", PlannedBudget = 450 };

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Title length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Valid Category", PlannedBudget = 5000 };
            _mockUnitOfWork.Setup(u => u.ExpenseCategories.AddAsync(It.IsAny<ExpenseCategory>()))
                           .Returns(Task.CompletedTask) // Simulate the async method without actual implementation
                           .Verifiable("AddAsync was not called with an ExpenseCategory object.");
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                           .ReturnsAsync(1) // Simulate saving changes successfully
                           .Verifiable("CompleteAsync was not called to save the changes.");

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Empty(errorMessages);

            _mockUnitOfWork.Verify(); // Verify that AddAsync and CompleteAsync were called as expected
        }

    }
}
