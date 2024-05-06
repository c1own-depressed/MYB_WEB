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
            _logger.LogInformation($"Trying to fetch incomes for user {userId}.");
            try
            {
                _logger.LogInformation($"Fetching incomes for user {userId}.");
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var incomes = await _unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);

                string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);

                // Generate all months within the date range
                var months = Enumerable.Range(0, ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month) + 1)
                    .Select(offset => new DateTime(startDate.Year, startDate.Month, 1).AddMonths(offset))
                    .ToList();

                // Initialize list to hold income data for all months
                List<IncomeDTO> incomeDTOs = new List<IncomeDTO>();

                // Populate income data for each month in the range
                foreach (var month in months)
                {
                    foreach (var income in incomes)
                    {
                        if (IsIncomeRelevantForMonth(income, month))
                        {
                            incomeDTOs.Add(new IncomeDTO
                            {
                                Id = income.Id,
                                IncomeName = income.IncomeName,
                                Amount = income.Amount,
                                CurrencyEmblem = currencyRepresentation,
                                Date = month, // Use the current month for this income
                                IsRegular = income.IsRegular,
                            });
                        }
                    }
                }

                // Group incomeDTOs by month and calculate total amount for each month
                var incomeByMonth = incomeDTOs
                    .GroupBy(income => new { Year = income.Date.Year, Month = income.Date.Month })
                    .Select(group => new IncomeStatisticDTO
                    {
                        Month = new DateTime(group.Key.Year, group.Key.Month, 1),
                        TotalAmount = group.Sum(income => income.Amount),
                    })
                    .OrderBy(dto => dto.Month);
                _logger.LogInformation($"Succesfully  fetched incomes.");
                return incomeByMonth;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income statistics.");
                throw; // Re-throw the exception for handling in the controller or elsewhere
            }
        }

        public async Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, string userId)
        {
            _logger.LogInformation($"Trying to fetch total expenses for user {userId}.");
            try
            {
                var categories = await _unitOfWork.ExpenseCategories.GetExpenseCategoriesByUserIdAsync(userId);
                var allExpenses = new List<Expense>();

                // Collect all expenses from all categories within the date range
                foreach (var category in categories)
                {
                    var expenses = await _unitOfWork.Expenses.GetAllExpensesByCategoryIdAndDateRangeAsync(category.Id, from, to);
                    allExpenses.AddRange(expenses);
                }

                // Group expenses by month and calculate the total amount per month
                var months = Enumerable.Range(0, int.MaxValue)
                    .Select(m => new DateTime(from.Year, from.Month, 1).AddMonths(m))
                    .TakeWhile(d => d <= new DateTime(to.Year, to.Month, 1))
                    .ToList();

                var expensesByMonth = allExpenses
                    .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                    .Select(group => new TotalExpensesDTO
                    {
                        Month = group.Key,
                        TotalAmount = group.Sum(e => e.Amount),
                    }).ToList();

                // Ensure all months are represented in the final data
                var groupedExpenses = months.Select(month => new TotalExpensesDTO
                {
                    Month = month,
                    TotalAmount = expensesByMonth.FirstOrDefault(e => e.Month == month)?.TotalAmount ?? 0,
                });

                _logger.LogInformation($"Successfully fetched total expenses for user {userId}.");
                return groupedExpenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving total expenses for user {userId}.");
                throw; // Re-throw the exception for handling in the controller or elsewhere
            }
        }

        public async Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, string userId)
        {
            _logger.LogInformation($"Calculating saved statistics for user {userId}.");
            try
            {
                var incomes = await GetIncomesByDate(from, to, userId);
                var expenses = await GetTotalExpensesByDate(from, to, userId);

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

                var savings = combinedData.Select(data => new SavedStatisticDTO
                {
                    Month = data.Month,
                    TotalAmount = data.IncomeAmount - data.ExpenseAmount,
                });

                _logger.LogInformation($"Successfully calculated saved statistics for user {userId}.");
                return savings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating saved statistics for user {userId}.");
                throw; // Re-throw the exception for handling in the controller or elsewhere
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

        private bool IsIncomeRelevantForMonth(Income income, DateTime month)
        {
            if (income.IsRegular)
            {
                // Regular income is relevant for the specified month
                return true;
            }
            else
            {
                // One-time income is relevant if its date falls within the specified month
                return income.Date.Year == month.Year && income.Date.Month == month.Month;
            }
        }
    }
}
