using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class IncomeRepository : RepositoryBase<Income>, IIncomeRepository
    {
        public IncomeRepository(MYBDbContext context) : base(context)
        {
        }

        public IEnumerable<Income> GetIncomesByUserId(int userId)
        {
            return _context.Set<Income>().Where(income => income.UserId == userId).ToList();
        }

        //public IEnumerable<Income> GetIncomes()
        //{
        //    return _context.Set<Income>().ToList();
        //}

        //public Income GetIncomeByID(int id)
        //{
        //    return _context.Set<Income>().Find(keyValues: id);
        //}

        //public void InsertIncome(Income income)
        //{
        //    _context.Set<Income>().Add(income);
        //}

        //public void DeleteIncome(int incomeID)
        //{
        //    Income income = _context.Set<Income>().Find(incomeID);
        //    _context.Set<Income>().Remove(income);
        //}

        //public void UpdateIncome(Income income)
        //{
        //    _context.Entry(income).State = EntityState.Modified;
        //}

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}
