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

        public async Task<IEnumerable<IncomeStatisticDTO>> getIncomesByDate (DateTime startDate, DateTime endDate, int UserId)
        {
            var user = await this._unitOfWork.Users.GetByIdAsync(UserId);
            var incomes = await this._unitOfWork.Incomes.GetIncomesByUserIdAsync(UserId);

            string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);

            var incomeDTOs = incomes
                .Where(income => (income.Date >= startDate && income.Date <= endDate) || income.IsRegular)
                .Select(income => new IncomeDTO
                {
                    Id = income.Id,
                    IncomeName = income.IncomeName,
                    Amount = income.Amount,
                    CurrencyEmblem = currencyRepresentation,
                    Date = income.Date,
                    IsRegular = income.IsRegular,
                });
            incomeDTOs.GroupBy(x => x.Date.Month);
            var incomeByMonth = incomeDTOs
                .GroupBy(income => new { Year = income.Date.Year, Month = income.Date.Month })
                .Select(group => new IncomeStatisticDTO
                {
                    Month = new DateTime(group.Key.Year, group.Key.Month, 1),
                    TotalAmount = group.Sum(income => income.Amount),
                })
                .OrderBy(dto => dto.Month);
            return incomeByMonth;
        }

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
