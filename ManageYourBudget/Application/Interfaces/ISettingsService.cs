using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISettingsService
    {
        Task<SettingsDTO> GetUserSettingsAsync(int userId);

        Task SaveSettings(SettingsDTO settingsDTO);
    }
}
