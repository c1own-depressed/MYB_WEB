using Application.DTOs.StatisticDTO;

namespace Application.Interfaces
{
    public interface IStatisticService
    {
        Task<IEnumerable<TotalExpensesDTO>> GetTotalExpensesByDate(DateTime from, DateTime to, int userId);
    }
}
