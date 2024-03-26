using Application.DTOs;
using Application.DTOs.Validators;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IncomeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
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

            await this._unitOfWork.Incomes.AddAsync(income);
            await this._unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(int userId)
        {
            var incomes = await this._unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);

            var incomeDTOs = incomes.Select(income => new IncomeDTO
            {
                Id = income.Id,
                IncomeName = income.IncomeName,
                Amount = income.Amount,
            });

            return incomeDTOs;
        }

        public async Task<ServiceResult> EditIncomeAsync(EditIncomeDTO model)
        {
            var validator = new EditIncomeDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var income = await this._unitOfWork.Incomes.GetByIdAsync(model.Id);

            if (income == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Income not found." });
            }

            // Update expenseCategory properties with values from DTO
            income.IncomeName = model.Name;
            income.Amount = model.Amount;

            await this._unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> RemoveIncomeAsync(int incomeId)
        {
            var incomeToRemove = await _unitOfWork.Incomes.GetByIdAsync(incomeId);

            if (incomeToRemove == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Income not found." });
            }

            // Delete the income
            this._unitOfWork.Incomes.Delete(incomeToRemove);
            await this._unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }
    }
}
