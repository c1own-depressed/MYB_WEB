using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User> GetByUsernameAsync(string username);

        Task<User> GetByEmailAsync(string email);

        Task AddAsync(User user);
    }
}
