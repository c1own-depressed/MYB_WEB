using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ThemeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ThemeService(IUnitOfWork unitOfWork, ILogger<ThemeService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> GetUserThemeAsync(string userId)
        {
            var sessionKey = $"theme_{userId}";
            var theme = _httpContextAccessor.HttpContext.Session.GetString(sessionKey);

            if (string.IsNullOrEmpty(theme))
            {
                _logger.LogInformation($"Theme not found in session for user {userId}, fetching from database.");
                try
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId) as Domain.Entities.User;
                    if (user != null)
                    {
                        theme = user.IsLightTheme.ToString();
                        _httpContextAccessor.HttpContext.Session.SetString(sessionKey, theme);
                    }
                    else
                    {
                        theme = true.ToString(); // Default to true if no user is found
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while fetching theme for user {userId}.");
                    throw;
                }
            }

            return bool.Parse(theme);
        }

        public async Task SaveUserThemeAsync(string userId, bool isLightTheme)
        {
            _logger.LogInformation($"Saving theme for user {userId}.");
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId) as Domain.Entities.User;
                if (user != null)
                {
                    user.IsLightTheme = isLightTheme;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.CompleteAsync();
                    _httpContextAccessor.HttpContext.Session.SetString($"theme_{userId}", isLightTheme.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while saving theme for user {userId}.");
                throw;
            }
        }
    }
}
