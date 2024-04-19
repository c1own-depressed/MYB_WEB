using Domain.Entities;
using Domain.Interfaces.Repositories;

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