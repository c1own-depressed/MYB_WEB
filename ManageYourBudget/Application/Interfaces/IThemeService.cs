using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IThemeService
    {
        Task<bool> GetUserThemeAsync(string userId);

        Task SaveUserThemeAsync(string userId, bool isLightTheme);
    }
}
