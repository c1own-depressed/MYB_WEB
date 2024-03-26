using Application.DTOs;
using Application.DTOs.Validators;
using Application.Interfaces;
using Application.Utils;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class SavingsService : ISavingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SavingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SavingsDTO>> GetSavingsByUserIdAsync(int userId)
        {
            var savings = await _unitOfWork.Savings.GetSavingsByUserIdAsync(userId);
            var savingsDTOs = savings.Select(s => new SavingsDTO
            {
                Id = s.Id,
                SavingsName = s.SavingsName,
                Amount = s.Amount,
            });
            return savingsDTOs;
        }

        public async Task<ServiceResult> AddSavingsAsync(CreateSavingsDTO model)
        {
            var validator = new CreateSavingsDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var savings = new Savings
            {
                UserId = 1, // TODO: Use actual user ID from session or request
                SavingsName = model.SavingsName,
                Amount = model.Amount,
            };

            await _unitOfWork.Savings.AddAsync(savings);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> RemoveSavingsAsync(int savingsId)
        {
            var savingsToRemove = await _unitOfWork.Savings.GetByIdAsync(savingsId);

            if (savingsToRemove == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Savings not found." });
            }

            _unitOfWork.Savings.Delete(savingsToRemove);
            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }

        public async Task<ServiceResult> EditSavingsAsync(EditSavingsDTO model)
        {
            var validator = new EditSavingsDTOValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var savings = await _unitOfWork.Savings.GetByIdAsync(model.Id);

            if (savings == null)
            {
                return new ServiceResult(success: false, errors: new[] { "Savings not found." });
            }

            // Update savings properties with values from DTO
            savings.SavingsName = model.SavingsName;
            savings.Amount = model.Amount;

            await _unitOfWork.CompleteAsync();

            return new ServiceResult(success: true);
        }
    }
}
