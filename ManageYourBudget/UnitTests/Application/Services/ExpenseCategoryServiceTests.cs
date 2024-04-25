using Application.DTOs.ExpenseDTOs;
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
            string testUserId = Guid.NewGuid().ToString();
            string categoryId1 = Guid.NewGuid().ToString();
            string categoryId2 = Guid.NewGuid().ToString();

            var testCategories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = categoryId1, CategoryName = "Test Category 1", Amount = 100, UserId = testUserId },
                new ExpenseCategory { Id = categoryId2, CategoryName = "Test Category 2", Amount = 200, UserId = testUserId }
            };

            var testExpenses = new List<ExpenseDTO>
            {
                new ExpenseDTO { Id = Guid.NewGuid().ToString(), ExpenseName = "Expense 1", Amount = 50, CategoryId = categoryId1 },
                new ExpenseDTO { Id = Guid.NewGuid().ToString(), ExpenseName = "Expense 2", Amount = 75, CategoryId = categoryId2 }
            };

            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetExpenseCategoriesByUserId(testUserId))
               .Returns(testCategories);


            _mockExpenseService.Setup(s => s.GetExpensesByCategoryIdAsync(Guid.NewGuid().ToString()))
                               .ReturnsAsync(testExpenses);

            // Act
            var result = await _service.GetExpenseCategoriesByUserIdAsync(testUserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testCategories.Count, result.Count());
        }

        [Fact]
        public async Task AddExpenseCategoryAsync_WithNegativeBudget_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseCategoryDTO { Title = "Negative Budget Category", PlannedBudget = -100 };
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);

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
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);

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
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);

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
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);
            
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
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);

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
            string userId = Guid.NewGuid().ToString();

            // Act
            var serviceResult = await _service.AddExpenseCategoryAsync(dto, userId);

            // Assert
            Assert.True(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Empty(errorMessages);

            _mockUnitOfWork.Verify(); // Verify that AddAsync and CompleteAsync were called as expected
        }

        [Fact]
        public async Task RemoveExpenseCategoryAsync_ExistingCategory_RemovesCategoryAndExpenses()
        {
            // Arrange
            string categoryId = Guid.NewGuid().ToString();
            var categoryToRemove = new ExpenseCategory { Id = categoryId, UserId = Guid.NewGuid().ToString() };
            var expensesToRemove = new List<Expense> { new Expense { Id = Guid.NewGuid().ToString(), CategoryId = categoryId } };

            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetByIdAsync(categoryId))
                           .ReturnsAsync(categoryToRemove);
            _mockUnitOfWork.Setup(u => u.Expenses.GetExpensesByCategoryIdAsync(categoryId))
                           .ReturnsAsync(expensesToRemove);

            // Act
            var serviceResult = await _service.RemoveExpenseCategoryAsync(categoryId);

            // Assert
            Assert.True(serviceResult.Success);
            Assert.Empty(serviceResult.Errors);

            _mockUnitOfWork.Verify(u => u.Expenses.Delete(It.IsAny<Expense>()), Times.Exactly(expensesToRemove.Count));
            _mockUnitOfWork.Verify(u => u.ExpenseCategories.Delete(categoryToRemove), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveExpenseCategoryAsync_NonExistingCategory_ReturnsError()
        {
            // Arrange
            string categoryId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetByIdAsync(categoryId))
                           .ReturnsAsync((ExpenseCategory)null);

            // Act
            var serviceResult = await _service.RemoveExpenseCategoryAsync(categoryId);

            // Assert
            Assert.False(serviceResult.Success);
            Assert.Collection(serviceResult.Errors, error => Assert.Equal("Expense category not found.", error));

            _mockUnitOfWork.Verify(u => u.Expenses.Delete(It.IsAny<Expense>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.ExpenseCategories.Delete(It.IsAny<ExpenseCategory>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }


        [Fact]
        public async Task EditExpenseCategoryAsync_WithNegativeBudget_ReturnsError()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "Negative Budget Category", PlannedBudget = -100 };

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task EditExpenseCategoryAsync_WithZeroBudget_ReturnsError()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "Zero Budget Category", PlannedBudget = 0 };

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be greater than 0.", errorMessages);
        }

        [Fact]
        public async Task EditExpenseCategoryAsync_WithExtremelyHighBudget_ReturnsError()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "High Budget Category", PlannedBudget = double.MaxValue };

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Planned budget must be lower than 100000000.", errorMessages);
        }

        [Fact]
        public async Task EditExpenseCategoryAsync_WithShortName_ReturnsError()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "Cate", PlannedBudget = 450 };

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Name length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task EditExpenseCategoryAsync_WithLongName_ReturnsError()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "Category New Category New Category New Category New Category New Category New Category New Category New Category New Category New", PlannedBudget = 450 };

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.False(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Equal("Name length should be between 5 and 100 characters.", errorMessages);
        }

        [Fact]
        public async Task EditExpenseCategoryAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var categoryId = Guid.NewGuid().ToString();
            var dto = new EditExpenseCategoryDTO { Id = categoryId, Name = "Valid Category", PlannedBudget = 5000 };
            _mockUnitOfWork.Setup(u => u.ExpenseCategories.GetByIdAsync(categoryId))
                           .ReturnsAsync(new ExpenseCategory { Id = categoryId, UserId = Guid.NewGuid().ToString() }); // Simulate getting an existing category
            _mockUnitOfWork.Setup(u => u.CompleteAsync())
                           .ReturnsAsync(1); // Simulate saving changes successfully

            // Act
            var serviceResult = await _service.EditExpenseCategoryAsync(dto);

            // Assert
            Assert.True(serviceResult.Success);
            var errorMessages = string.Join("; ", serviceResult.Errors);
            Assert.Empty(errorMessages);
        }
    }
}
