using Application.DTOs.StatisticDTO;
namespace Application.Interfaces
{
    public interface IStatisticService
    {

        Task<IEnumerable<IncomeStatisticDTO>> getIncomesByDate(DateTime startMonth, DateTime endMonth, int UserId);
        Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, int userId);
        Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, int userId);

    }
}
