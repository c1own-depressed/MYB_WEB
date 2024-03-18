

using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal class SavingsRepository : RepositoryBase<Savings>, ISavingsRepository
    {
        public SavingsRepository(DbContext context) : base(context)
        {
        }

        public IEnumerable<Savings> GetSavingsByUserId(int userId)
        {
            return _context.Set<Savings>().Where(savings => savings.UserId == userId).ToList();
        }

        //public IEnumerable<Savings> GetSavings()
        //{
        //    return _context.Set<Savings>().ToList();
        //}

        //public Savings GetSavingsByID(int id)
        //{
        //    return _context.Set<Savings>().Find(keyValues: id);
        //}

        //public void InsertSavings(Savings savings)
        //{
        //    _context.Set<Savings>().Add(savings);
        //}

        //public void DeleteSavings(int savingsID)
        //{
        //    Savings savings = _context.Set<Savings>().Find(savingsID);
        //    _context.Set<Savings>().Remove(savings);
        //}

        //public void UpdateSavings(Savings savings)
        //{
        //    _context.Entry(savings).State = EntityState.Modified;
        //}

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}