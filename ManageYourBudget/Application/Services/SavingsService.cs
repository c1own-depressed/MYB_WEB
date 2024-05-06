using Application.DTOs.SavingsDTOs;
using Application.Interfaces;
using Application.Utils;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Application.Services
{
    public class SavingsService : ISavingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SavingsService> _logger;
        public SavingsService(IUnitOfWork unitOfWork, ILogger<SavingsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<SavingsDTO>> GetSavingsByUserIdAsync(string userId)
        {
            _logger.LogInformation($"Fetching savings for user {userId}.");
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var savings = await _unitOfWork.Savings.GetSavingsByUserIdAsync(userId);

                string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);

                var savingsDTOs = savings.Select(s => new SavingsDTO
                {
                    Id = s.Id,
                    SavingsName = s.SavingsName,
                    Amount = s.Amount,
                    CurrencyEmblem = currencyRepresentation,
                    UserId = userId,
                });

                _logger.LogInformation($"Successfully fetched savings for user {userId}.");
                return savingsDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching savings for user {userId}.");
                throw;
            }
        }

        public async Task<ServiceResult> AddSavingsAsync(CreateSavingsDTO model, string userId)
        {
            _logger.LogInformation($"Adding savings for user {userId}.");
            try
            {
                var validator = new CreateSavingsDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var savings = new Savings
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Date = model.Date,
                    SavingsName = model.SavingsName,
                    Amount = model.Amount,
                };

                await _unitOfWork.Savings.AddAsync(savings);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully added savings for user {userId}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding savings for user {userId}.");
                throw;
            }
        }

        public async Task<ServiceResult> RemoveSavingsAsync(string savingsId)
        {
            _logger.LogInformation($"Removing savings with ID: {savingsId}.");
            try
            {
                var savingsToRemove = await _unitOfWork.Savings.GetByIdAsync(savingsId);

                if (savingsToRemove == null)
                {
                    return new ServiceResult(success: false, errors: new[] { "Savings not found." });
                }

                _unitOfWork.Savings.Delete(savingsToRemove);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully removed savings with ID: {savingsId}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing savings with ID: {savingsId}.");
                throw;
            }
        }

        public async Task<ServiceResult> EditSavingsAsync(EditSavingsDTO model)
        {
            _logger.LogInformation($"Editing savings with ID: {model.Id}.");
            try
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

                _logger.LogInformation($"Successfully edited savings with ID: {model.Id}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing savings with ID: {model.Id}.");
                throw;
            }
        }
    }
}