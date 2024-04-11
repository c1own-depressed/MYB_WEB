using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Interfaces;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341"));

var connectionString = builder.Configuration.GetConnectionString("DimaConnection");

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

// If you have interfaces for your repositories, register them here
// builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();

// Register your application services
builder.Services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<ISavingsService, SavingsService>();
builder.Services.AddControllersWithViews()
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateExpenseCategoryDTOValidator>());
builder.Services.AddControllersWithViews()
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<EditExpenseCategoryDTOValidator>());
builder.Services.AddControllersWithViews()
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateSavingsDTOValidator>());
builder.Services.AddControllersWithViews()
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<EditSavingsDTOValidator>());
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();

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

app.Run();
