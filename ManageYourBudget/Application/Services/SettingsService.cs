using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<SettingsService> _logger;

        public SettingsService(IUnitOfWork unitOfWork, ILogger<SettingsService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SettingsDTO> GetUserSettingsAsync(string userId)
        {
            _logger.LogInformation($"Fetching settings for user {userId}.");
            try
            {
                var userFromDb = await this._unitOfWork.Users.GetByIdAsync(userId);
                if (userFromDb != null)
                {
                    SettingsDTO dto = new SettingsDTO
                    {
                        Id = userFromDb.Id,
                        Currency = userFromDb.Currency,
                        IsLightTheme = userFromDb.IsLightTheme,
                        Language = userFromDb.Language,
                    };

                    return dto;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching settings for user {userId}.");
                throw;
            }

            return GetDefaultSettings();
        }

        public async Task SaveSettings(SettingsDTO settingsDTO)
        {
            _logger.LogInformation($"Saving settings for user {settingsDTO.Id}.");
            try
            {
                // Отримуємо користувача з бази даних за його Id
                var user = await this._unitOfWork.Users.GetByIdAsync(settingsDTO.Id);

                // Оновлюємо налаштування користувача з даних DTO
                if (user != null)
                {
                    user.Language = settingsDTO.Language;
                    user.IsLightTheme = settingsDTO.IsLightTheme;
                    user.Currency = settingsDTO.Currency;

                    // Оновлюємо користувача в базі даних
                    this._unitOfWork.Users.Update(user);
                    await this._unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while saving settings for user {settingsDTO.Id}.");
                throw;
            }
        }

        // TODO: shouldn't be used
        private SettingsDTO GetDefaultSettings()
        {
            // Initialize with default values
            return new SettingsDTO
            {
                Id = string.Empty,
                Currency = "USD",
                IsLightTheme = true,
                Language = "English",
            };
        }
    }
}
