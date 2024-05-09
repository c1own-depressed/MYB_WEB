using Application.DTOs.IncomeDTOs;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StatisticService> _logger;

        public StatisticService(IUnitOfWork unitOfWork, ILogger<StatisticService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<IncomeStatisticDTO>> GetIncomesByDate(DateTime startDate, DateTime endDate, string userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var incomes = await _unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);
                string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);
                var months = Enumerable.Range(0, ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month) + 1)
                                       .Select(offset => new DateTime(startDate.Year, startDate.Month, 1).AddMonths(offset))
                                       .ToList();

                var monthlyIncome = new Dictionary<DateTime, double>();
                foreach (var month in months)
                {
                    monthlyIncome[month] = 0;  // Initialize all months with zero
                }

                foreach (var income in incomes)
                {
                    var incomeMonth = new DateTime(income.Date.Year, income.Date.Month, 1);
                    if (incomeMonth >= startDate && incomeMonth <= endDate && IsIncomeRelevantForMonth(income, incomeMonth))
                    {
                        monthlyIncome[incomeMonth] += income.Amount;
                    }
                }

                var incomeByMonth = monthlyIncome.Select(kvp => new IncomeStatisticDTO
                {
                    Month = kvp.Key,
                    TotalAmount = kvp.Value
                }).OrderBy(dto => dto.Month);

                _logger.LogInformation("Successfully fetched incomes for user {UserId}.", userId);
                return incomeByMonth;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch incomes for user {UserId}.", userId);
                throw;
            }
        }

        private bool IsIncomeRelevantForMonth(Income income, DateTime month)
        {
            // Check if the income should be counted in the specified month
            if (income.IsRegular)
            {
                // Regular income counts every month
                return true;
            }
            else
            {
                // One-time income only counts in the month it was received
                return income.Date.Month == month.Month && income.Date.Year == month.Year;
            }
        }

        public async Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, string userId)
        {
            try
            {
                var categories = await _unitOfWork.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId);
                var allExpenses = new List<Expense>();

                foreach (var category in categories)
                {
                    var expenses = await _unitOfWork.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(category.Id, from, to);
                    allExpenses.AddRange(expenses);
                }

                var months = Enumerable.Range(0, int.MaxValue)
                                       .Select(m => new DateTime(from.Year, from.Month, 1).AddMonths(m))
                                       .TakeWhile(d => d <= new DateTime(to.Year, to.Month, 1))
                                       .ToList();

                var expensesByMonth = allExpenses.GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                                                 .Select(group => new TotalExpensesDTO
                                                 {
                                                     Month = group.Key,
                                                     TotalAmount = group.Sum(e => e.Amount),
                                                 }).ToList();

                var groupedExpenses = months.Select(month => new TotalExpensesDTO
                {
                    Month = month,
                    TotalAmount = expensesByMonth.FirstOrDefault(e => e.Month == month)?.TotalAmount ?? 0,
                });

                _logger.LogInformation("Successfully fetched expenses for user {UserId}.", userId);
                return groupedExpenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch expenses for user {UserId}.", userId);
                throw;
            }
        }

        public async Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, string userId)
        {
            try
            {
                var incomes = await GetIncomesByDate(from, to, userId);
                var expenses = await GetTotalExpensesByDate(from, to, userId);

                var savings = incomes.Join(
                    expenses,
                    income => income.Month,
                    expense => expense.Month,
                    (income, expense) => new
                    {
                        Month = income.Month,
                        IncomeAmount = income.TotalAmount,
                        ExpenseAmount = expense.TotalAmount,
                    })
                .Select(data => new SavedStatisticDTO
                {
                    Month = data.Month,
                    TotalAmount = data.IncomeAmount - data.ExpenseAmount,
                }).ToList();

                _logger.LogInformation("Successfully calculated savings for user {UserId}.", userId);
                return savings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate savings for user {UserId}.", userId);
                throw;  // It's typically a good idea to re-throw the exception unless you have a specific reason to swallow it.
            }
        }

        public async Task<AllStatisticDataDTO> GetAllData(DateTime startDate, DateTime endDate, string userId)
        {
            _logger.LogInformation($"Trying to fetch all data for user {userId}.");
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
                _logger.LogInformation($"Successfully fetched all data.");
                return allData;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors
                _logger.LogError(ex, $"Error fetching all data for user {userId}.");
                throw; // Re-throw exception or return default value as needed
            }
        }
    }
}
