using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Persistence.Data;
using Domain.Interfaces.Repositories;

namespace Persistence.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(MYBDbContext context) : base(context)
        {
        }

        //public IEnumerable<User> GetUsers()
        //{
        //    return _context.Set<User>().ToList();
        //}

        //public User GetUserByID(int id)
        //{
        //    return _context.Set<User>().Find(keyValues: id);
        //}

        //public void InsertUser(User user)
        //{
        //    _context.Set<User>().Add(user);
        //}

        //public void DeleteUser(int userID)
        //{
        //    User user = _context.Set<User>().Find(userID);
        //    _context.Set<User>().Remove(user);
        //}

        //public void UpdateUser(User user)
        //{
        //    _context.Entry(user).State = EntityState.Modified;
        //}

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}
    }
}