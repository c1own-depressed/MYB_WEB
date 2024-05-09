using Microsoft.AspNetCore.Localization;
using Application.Interfaces;
using System.Security.Claims;

public class DbRequestCultureProvider : RequestCultureProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DbRequestCultureProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var cultureService = scope.ServiceProvider.GetRequiredService<ICultureService>();
            string userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return null;

            string culture = await cultureService.GetUserCultureAsync(userId);
            if (string.IsNullOrEmpty(culture))
                return null;

            return new ProviderCultureResult(culture, culture);
        }
    }
}
