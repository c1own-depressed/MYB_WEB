using Application.DTOs.IncomeDTOs;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;

        public async Task<IEnumerable<IncomeStatisticDTO>> GetIncomesByDate (DateTime startDate, DateTime endDate, string userId)
        {
            var user = await this._unitOfWork.Users.GetByIdAsync(userId);
            var incomes = await this._unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);

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

        public async Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, string userId)
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

        public async Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, string userId)
        {
            var incomes = await GetIncomesByDate(from, to, userId);

            var expenses = await GetTotalExpensesByDate(from, to, userId);

            var savings = new List<SavedStatisticDTO>();

            var combinedData = incomes.Join(
                expenses,
                income => income.Month,
                expense => expense.Month,
                (income, expense) => new
                {
                    Month = income.Month,
                    IncomeAmount = income.TotalAmount,
                    ExpenseAmount = expense.TotalAmount,
                });

            foreach (var data in combinedData)
            {
                var savedAmount = new SavedStatisticDTO
                {
                    Month = data.Month,
                    TotalAmount = data.IncomeAmount - data.ExpenseAmount,
                };
                savings.Add(savedAmount);
            }

            return savings;
        }

        public async Task<AllStatisticDataDTO> GetAllData(DateTime startDate, DateTime endDate, string userId)
        {
            try
            {
                // Retrieve income statistics
                var incomeStatistics = await GetIncomesByDate(startDate, endDate, userId);

                // Retrieve total expenses statistics
                var expensesStatistics = await GetTotalExpensesByDate(startDate, endDate, userId);

                // Retrieve savings statistics
                var savingsStatistics = await CountSaved(startDate, endDate, userId);

                // Assemble all statistics into a single DTO
                var allData = new AllStatisticDataDTO
                {
                    IncomeStatistics = incomeStatistics.ToList(),
                    ExpensesStatistics = expensesStatistics.ToList(),
                    SavingsStatistics = savingsStatistics.ToList(),
                };

                return allData;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                Console.WriteLine($"Error retrieving all statistics: {ex.Message}");
                throw; // Re-throw exception or return default value as needed
            }
        }
    }
}

