using Application.DTOs.IncomeDTOs;
using Application.Interfaces;
using Application.Utils;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IncomeService> _logger;

        public IncomeService(IUnitOfWork unitOfWork, ILogger<IncomeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult> AddIncomeAsync(CreateIncomeDTO model, string userId)
        {
            _logger.LogInformation($"Adding income for user {userId}.");
            try
            {
                var validator = new CreateIncomeDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var income = new Income
                {
                    Id = Guid.NewGuid().ToString(),
                    IncomeName = model.IncomeName,
                    Amount = model.Amount,
                    IsRegular = model.IsRegular,
                    Date = model.Date,
                    UserId = userId,
                };

                await _unitOfWork.Incomes.AddAsync(income);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully added income for user {userId}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding income for user {userId}.");
                throw;
            }
        }

        public async Task<IEnumerable<IncomeDTO>> GetIncomesByUserIdAsync(string userId)
        {
            _logger.LogInformation($"Fetching incomes for user {userId}.");
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                var incomes = await _unitOfWork.Incomes.GetIncomesByUserIdAsync(userId);

                string currencyRepresentation = CurrencyUtils.FormatCurrencyDisplay(user.Currency);

                var incomeDTOs = incomes.Select(income => new IncomeDTO
                {
                    Id = income.Id,
                    IncomeName = income.IncomeName,
                    Amount = income.Amount,
                    CurrencyEmblem = currencyRepresentation,
                    Date = income.Date,
                    IsRegular = income.IsRegular,
                });

                _logger.LogInformation($"Successfully fetched incomes for user {userId}.");
                return incomeDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching incomes for user {userId}.");
                throw;
            }
        }

        public async Task<ServiceResult> EditIncomeAsync(EditIncomeDTO model)
        {
            _logger.LogInformation($"Editing income with ID: {model.Id}.");
            try
            {
                var validator = new EditIncomeDTOValidator();
                var validationResult = validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    return new ServiceResult(success: false, errors: validationResult.Errors.Select(e => e.ErrorMessage));
                }

                var income = await _unitOfWork.Incomes.GetByIdAsync(model.Id);

                if (income == null)
                {
                    return new ServiceResult(success: false, errors: new[] { "Income not found." });
                }

                // Update income properties with values from DTO
                income.IncomeName = model.Name;
                income.Amount = model.Amount;

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully edited income with ID: {model.Id}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing income with ID: {model.Id}.");
                throw;
            }
        }

        public async Task<ServiceResult> RemoveIncomeAsync(string incomeId)
        {
            _logger.LogInformation($"Removing income with ID: {incomeId}.");
            try
            {
                var incomeToRemove = await _unitOfWork.Incomes.GetByIdAsync(incomeId);

                if (incomeToRemove == null)
                {
                    return new ServiceResult(success: false, errors: new[] { "Income not found." });
                }

                // Delete the income
                _unitOfWork.Incomes.Delete(incomeToRemove);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully removed income with ID: {incomeId}.");
                return new ServiceResult(success: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing income with ID: {incomeId}.");
                throw;
            }
        }
    }
}
