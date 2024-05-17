using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using System.Security.Claims;

public class DbRequestCultureProvider : RequestCultureProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DbRequestCultureProvider> _logger;

    public DbRequestCultureProvider(IServiceScopeFactory scopeFactory, ILogger<DbRequestCultureProvider> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        _logger.LogInformation("Starting to determine the culture result.");

        using (var scope = _scopeFactory.CreateScope())
        {
            var cultureService = scope.ServiceProvider.GetRequiredService<ICultureService>();
            string userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("User ID is null or empty, cannot determine culture.");
                return new ProviderCultureResult("en-US", "en-US");
            }

            _logger.LogInformation("Retrieving culture for user ID: {UserId}", userId);
            string culture = await cultureService.GetUserCultureAsync(userId);
            if (string.IsNullOrEmpty(culture))
            {
                _logger.LogInformation("No culture found for user ID: {UserId}", userId);
                return new ProviderCultureResult("en-US", "en-US");
            }

            _logger.LogInformation("Culture for user ID {UserId} determined as {Culture}", userId, culture);
            return new ProviderCultureResult(culture, culture);
        }
    }
}
