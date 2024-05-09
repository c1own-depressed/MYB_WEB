using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services
{
    public class CultureService : ICultureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CultureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetUserCultureAsync(string userId)
        {
            // Assuming you have a Users table with a Culture column
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user?.Language ?? "en-US"; // Default to English if not specified
        }
    }
}
