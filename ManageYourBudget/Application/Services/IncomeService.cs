using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;


namespace Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncomeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddIncome(Income income)
        {
            _unitOfWork.Incomes.AddAsync(income);
            await _unitOfWork.CompleteAsync();
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
