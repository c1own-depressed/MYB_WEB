using Application.DTOs.StatisticDTO;
namespace Application.Interfaces
{
    public interface IStatisticService
    {

        Task<IEnumerable<IncomeStatisticDTO>> getIncomesByDate(DateTime startMonth, DateTime endMonth, string UserId);
        Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, string userId);
        Task<IEnumerable<SavedStatisticDTO>> CountSaved(DateTime from, DateTime to, string userId);
        Task<AllStatisticDataDTO> GetAllData(DateTime startDate, DateTime endDate, string userId);

    }
}
