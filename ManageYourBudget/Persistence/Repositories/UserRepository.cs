using Domain.Entities;
using Domain.Interfaces.Repositories;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(MYBDbContext context)
            : base(context)
        {
        }
    }
}