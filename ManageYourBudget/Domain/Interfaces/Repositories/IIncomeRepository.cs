using Domain.Entities;


namespace Domain.Interfaces.Repositories
{
    public interface IIncomeRepository
    {
        IEnumerable<Income> GetIncomesByUserId(int userId);
        //IEnumerable<Income> GetIncomes();
        //Income GetIncomeByID(int incomeId);
        //void InsertIncome(Income income);
        //void DeleteIncome(int incomeID);
        //void UpdateIncome(Income income);
        //void Save();
    }
}
