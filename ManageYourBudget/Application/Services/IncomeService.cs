using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Reflection;


namespace Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool isSuccess, string errorMessage)> AddIncomeAsync(IncomeDTO model)
        {

            if (model.Amount <= 0)
            {
                return (false, "Planned budget must be greater than 0.");
            }
            if (model.Amount > 99999999)
            {
                return (false, "Planned budget must be lower than 100000000.");
            }
            if (model.IncomeName.Length < 5 || model.IncomeName.Length > 100)
            {
                return (false, "Title length should be between 5 and 100 characters.");
            }

            var income = new Income
            {
                UserId = 1, // TODO: Use actual user ID from session or request
                IncomeName = model.IncomeName,
                Amount = model.Amount,
            };

            await _unitOfWork.Incomes.AddAsync(income);
            await _unitOfWork.CompleteAsync();

            return (true, "");
        }

        public async Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(int userId)
        {
            var incomes = await _unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);

            var incomeDTOs = incomes.Select(income => new IncomeDTO
            {
                IncomeName = income.IncomeName,
                Amount = income.Amount,
            });

            return incomeDTOs;
        }
    }
}
