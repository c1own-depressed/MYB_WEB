using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Entities;
using Domain.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.AuthService;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

var connectionString = builder.Configuration.GetConnectionString("RomanConnection");

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

var cultures = new[] { "en", "uk" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(cultures[0])
    .AddSupportedCultures(cultures)
    .AddSupportedUICultures(cultures);
app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "faq",
    pattern: "faq",
    defaults: new { controller = "FAQPage", action = "Index" });

app.MapControllerRoute(
    name: "settings",
    pattern: "settings",
    defaults: new { controller = "SettingsPage", action = "Index" });

app.MapControllerRoute(
    name: "tips",
    pattern: "tips",
    defaults: new { controller = "Tips", action = "Index" });

app.MapControllerRoute(
    name: "statistic",
    pattern: "statistic",
    defaults: new { controller = "StatisticPage", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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


// TODO: after the application is deployed
// app.UseCors(options => options.WithOrigins("https://example.com")); // Adjust accordingly
app.Run();
