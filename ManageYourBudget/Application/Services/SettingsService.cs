using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SettingsDTO> GetUserSettingsAsync(int userId)
        {
            var userFromDb = await this._unitOfWork.Users.GetByIdAsync(userId);
            if (userFromDb != null)
            {
                SettingsDTO dto = new SettingsDTO
                {
                    Currency = userFromDb.Currency,
                    IsLightTheme = userFromDb.IsLightTheme,
                    Language = userFromDb.Language,
                };

                return dto;
            }

            // TODO: Service Result
            return null;
        }

        public async Task SaveSettings(SettingsDTO settingsDTO)
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
            else
            {
                // Якщо користувача не знайдено, можна обробити цю ситуацію відповідним чином, наприклад, кинути виключення або створити нового користувача.
                // Ваш код обробки тут.
            }
        }
    }
}
