using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class StatisticService : IStatisticService
    {

        private readonly IUnitOfWork _unitOfWork;

        public StatisticService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, int userId)
        {
            var categories = await _unitOfWork.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId);

            var allExpenses = new List<Expense>();

            // Collect all expenses from all categories within the date range
            foreach (var category in categories)
            {
                var expenses = await _unitOfWork.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(category.Id, from, to);
                allExpenses.AddRange(expenses);
            }

            // Group by Month and calculate the total amount per month
            var groupedExpenses = allExpenses
                .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                .Select(group => new TotalExpensesDTO
                {
                    Month = group.Key,
                    TotalAmount = group.Sum(e => e.Amount),
                });

            return groupedExpenses;
        }
    }
}
