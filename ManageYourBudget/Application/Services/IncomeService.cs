using Application.DTOs;
using Application.DTOs.Validators;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
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

        public async Task<ServiceResult> AddIncomeAsync(IncomeDTO model)
        {
            var validator = new IncomeDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var income = new Income
            {
                UserId = 1, // TODO: Use actual user ID from session or request
                IncomeName = model.IncomeName,
                Amount = model.Amount,
            };

            await _unitOfWork.Incomes.AddAsync(income);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
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
