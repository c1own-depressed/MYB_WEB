using Application.DTOs.StatisticDTO;

namespace Application.Interfaces
{
    public interface IStatisticService
    {
        Task<IEnumerable<IncomeStatisticDTO>> GetIncomesByDate(DateTime startMonth, DateTime endMonth, int userId);

        Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, int userId);

        Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, int userId);

        Task<AllStatisticDataDTO> GetAllData(DateTime startDate, DateTime endDate, int userId);
    }
}
