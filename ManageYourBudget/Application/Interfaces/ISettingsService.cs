using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISettingsService
    {
        Task<SettingsDTO> GetUserSettingsAsync(string userId);

        Task SaveSettings(SettingsDTO settingsDTO);
    }
}
