using Application.DTOs;
using Application.DTOs.ExpenseDTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Application.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ExpenseService _service;

        public ExpenseServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _service = new ExpenseService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task AddExpenseAsync_WithNegativeAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseDTO { ExpenseName = "Negative Expense", Amount = -100, CategoryId = Guid.NewGuid().ToString() };

            // Act
            var serviceResult = await _service.AddExpenseAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Expense amount must be greater than 0.");
        }

        [Fact]
        public async Task AddExpenseAsync_WithZeroAmount_ReturnsError()
        {
            // Arrange
            var dto = new CreateExpenseDTO { ExpenseName = "Zero Expense", Amount = 0, CategoryId = Guid.NewGuid().ToString() };

            // Act
            var serviceResult = await _service.AddExpenseAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Expense amount must be greater than 0.");
        }

        [Fact]
        public async Task AddExpenseAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateExpenseDTO { ExpenseName = "Valid Expense", Amount = 500, CategoryId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Expenses.AddAsync(It.IsAny<Expense>()))
                           .Returns(Task.CompletedTask); // Simulate saving successfully

            // Act
            var serviceResult = await _service.AddExpenseAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveExpenseAsync_ExistingExpense_RemovesExpense()
        {
            // Arrange
            string expenseId = Guid.NewGuid().ToString();
            var expenseToRemove = new Expense { Id = expenseId, CategoryId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Expenses.GetByIdAsync(expenseId))
                           .ReturnsAsync(expenseToRemove);

            // Act
            var serviceResult = await _service.RemoveExpenseAsync(expenseId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();

            _mockUnitOfWork.Verify(u => u.Expenses.Delete(expenseToRemove), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveExpenseAsync_NonExistingExpense_ReturnsError()
        {
            // Arrange
            string expenseId = Guid.NewGuid().ToString();

            _mockUnitOfWork.Setup(u => u.Expenses.GetByIdAsync(expenseId))
                           .ReturnsAsync((Expense)null);

            // Act
            var serviceResult = await _service.RemoveExpenseAsync(expenseId);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Expense not found.");

            _mockUnitOfWork.Verify(u => u.Expenses.Delete(It.IsAny<Expense>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task EditExpenseAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            string expenseId = Guid.NewGuid().ToString();
            var dto = new EditExpenseDTO { Id = expenseId, ExpenseName = "Edited Expense", Amount = 1000 };
            var expense = new Expense { Id = expenseId, ExpenseName = "Original Expense", Amount = 500, CategoryId = Guid.NewGuid().ToString() };

            _mockUnitOfWork.Setup(u => u.Expenses.GetByIdAsync(expenseId))
                           .ReturnsAsync(expense);

            // Act
            var serviceResult = await _service.EditExpenseAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeTrue();
            serviceResult.Errors.Should().BeEmpty();
            expense.ExpenseName.Should().Be(dto.ExpenseName);
            expense.Amount.Should().Be(dto.Amount);

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task EditExpenseAsync_NonExistingExpense_ReturnsError()
        {
            // Arrange
            string expenseId = Guid.NewGuid().ToString();
            var dto = new EditExpenseDTO { Id = expenseId, ExpenseName = "Edited Expense", Amount = 1000 };

            _mockUnitOfWork.Setup(u => u.Expenses.GetByIdAsync(expenseId))
                           .ReturnsAsync((Expense)null);

            // Act
            var serviceResult = await _service.EditExpenseAsync(dto);

            // Assert
            serviceResult.Should().NotBeNull();
            serviceResult.Success.Should().BeFalse();
            serviceResult.Errors.Should().Contain("Expense not found.");

            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
        }
    }
}
