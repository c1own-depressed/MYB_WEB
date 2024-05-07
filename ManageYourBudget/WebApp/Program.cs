using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Serilog;
using Persistence.AuthService;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

var connectionString = builder.Configuration.GetConnectionString("RostikConnection");


if (connectionString != null)
{
    builder.Services.AddDbContext<MYBDbContext>(options =>
        options.UseMySQL(connectionString));
}
else
{
    Log.Error("Connection string is null.");
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register your unit of work and repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();  // TODO: check if should depend on IUnitOfWork from Domain Layer

// Register your application services
builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<ISavingsService, SavingsService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICultureService, CultureService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateExpenseCategoryDTOValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<EditExpenseCategoryDTOValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<CreateSavingsDTOValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<EditSavingsDTOValidator>();
    });
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

// Add Identity services and configure the options
builder.Services.AddDefaultIdentity<User>(options => {
    options.SignIn.RequireConfirmedAccount = true; // Depends on your requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<MYBDbContext>(); // Link Identity to the EF Core store

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLoggedIn", policy =>
        policy.RequireAuthenticatedUser());
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // This is essential for Identity
app.UseAuthorization();

var supportedCultures = new[] { "en-US", "uk-UA" };
var requestLocalizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Configure the custom RequestCultureProvider using IServiceScopeFactory
requestLocalizationOptions.RequestCultureProviders.Insert(0,
    new DbRequestCultureProvider(app.Services.GetRequiredService<IServiceScopeFactory>()));

// Apply the localization settings to the application
app.UseRequestLocalization(requestLocalizationOptions);

app.MapControllerRoute(
    name: "faq",
    pattern: "faq",
    defaults: new { controller = "FAQPage", action = "Index" });

app.MapControllerRoute(
    name: "settings",
    pattern: "settings",
    defaults: new { controller = "SettingsPage", action = "Index" })
    .RequireAuthorization("RequireLoggedIn");

app.MapControllerRoute(
    name: "tips",
    pattern: "tips",
    defaults: new { controller = "Tips", action = "Index" });

app.MapControllerRoute(
    name: "statistic",
    pattern: "statistic",
    defaults: new { controller = "StatisticPage", action = "Index" })
    .RequireAuthorization("RequireLoggedIn");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tips}/{action=Index}");

app.MapControllerRoute(
    name: "home",
    pattern: "{controller=Home}/{action=Index}")
    .RequireAuthorization("RequireLoggedIn");

app.MapControllerRoute(
    name: "signup",
    pattern: "signup",
    defaults: new { controller = "Account", action = "Register" });

app.MapControllerRoute(
    name: "checkemail",
    pattern: "checkemail",
    defaults: new { controller = "Account", action = "CheckConfirm" });

app.MapControllerRoute(
    name: "confirmemail",
    pattern: "confirmemail",
    defaults: new { controller = "Account", action = "EmailConfirm" });

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "forgotpassword",
    pattern: "forgotpassword",
    defaults: new { controller = "Account", action = "ForgotPassword" });

app.MapControllerRoute(
    name: "resetpassword",
    pattern: "resetpassword",
    defaults: new { controller = "Account", action = "ResetPassword" });

// TODO: after the application is deployed
// app.UseCors(options => options.WithOrigins("https://example.com")); // Adjust accordingly
app.Run();
