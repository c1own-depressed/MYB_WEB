using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using ClosedXML.Excel;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Persistence.Services
{
    public class ExportDataService : IExportDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStatisticService _statisticService;
        private readonly ILogger<ExportDataService> _logger;

        public ExportDataService(IUnitOfWork unitOfWork, IStatisticService statisticService, ILogger<ExportDataService> logger)
        {
            _unitOfWork = unitOfWork;
            _statisticService = statisticService;
            _logger = logger;
        }

        public async Task<string> ExportDataToCSV(DateTime startDate, DateTime endDate, string userId)
        {
            _logger.LogInformation($"Exporting data to CSV for user ID: {userId}");

            try
            {
                var data = await _statisticService.GetAllData(startDate, endDate, userId);

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Month,Total Income,Total Expenses,Total Savings");

                data.IncomeStatistics ??= new List<IncomeStatisticDTO>();
                data.ExpensesStatistics ??= new List<TotalExpensesDTO>();
                data.SavingsStatistics ??= new List<SavedStatisticDTO>();

                for (int i = 0; i < data.IncomeStatistics.Count; i++)
                {
                    var income = data.IncomeStatistics[i];
                    var expense = data.ExpensesStatistics.ElementAtOrDefault(i);
                    var saving = data.SavingsStatistics.ElementAtOrDefault(i);

                    double totalExpenses = expense?.TotalAmount ?? 0;
                    double totalSavings = saving?.TotalAmount ?? 0;
                    string formattedMonth = income.Month.ToString("MMMM yyyy");

                    stringBuilder.AppendLine($"{formattedMonth},{income.TotalAmount},{totalExpenses},{totalSavings}");
                }

                _logger.LogInformation($"CSV export completed successfully for user ID: {userId}");

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during CSV export for user ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<byte[]> ExportDataToXML(DateTime startDate, DateTime endDate, string userId)
        {
            _logger.LogInformation($"Exporting data to XML for user ID: {userId}");

            try
            {
                var data = await _statisticService.GetAllData(startDate, endDate, userId);

                var xDocument = new XDocument(new XElement(
                    "Statistics",
                    new XElement("Incomes", data.IncomeStatistics.Select(income => new XElement("Income", new XElement("Month", income.Month.ToString("MMMM yyyy")), new XElement("TotalAmount", income.TotalAmount)))),
                    new XElement("Expenses", data.ExpensesStatistics.Select(expense => new XElement("Expense", new XElement("Month", expense.Month.ToString("MMMM yyyy")), new XElement("TotalAmount", expense.TotalAmount)))),
                    new XElement("Savings", data.SavingsStatistics.Select(saving => new XElement("Saving", new XElement("Month", saving.Month.ToString("MMMM yyyy")), new XElement("TotalAmount", saving.TotalAmount))))));

                using (var memoryStream = new MemoryStream())
                {
                    xDocument.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during XML export for user ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<byte[]> ExportDataToXLSX(DateTime startDate, DateTime endDate, string userId)
        {
            _logger.LogInformation($"Exporting data to XLSX for user ID: {userId}");

            try
            {
                var data = await _statisticService.GetAllData(startDate, endDate, userId);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.AddWorksheet("Statistics");
                    worksheet.Cell("A1").Value = "Month";
                    worksheet.Cell("B1").Value = "Total Income";
                    worksheet.Cell("C1").Value = "Total Expenses";
                    worksheet.Cell("D1").Value = "Total Savings";

                    int row = 2;
                    foreach (var income in data.IncomeStatistics)
                    {
                        var expense = data?.ExpensesStatistics?.FirstOrDefault(e => e.Month == income.Month);
                        var saving = data?.SavingsStatistics?.FirstOrDefault(s => s.Month == income.Month);

                        worksheet.Cell(row, 1).Value = income.Month.ToString("MMMM yyyy");
                        worksheet.Cell(row, 2).Value = income.TotalAmount;
                        worksheet.Cell(row, 3).Value = expense?.TotalAmount ?? 0;
                        worksheet.Cell(row, 4).Value = saving?.TotalAmount ?? 0;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred during XLSX export for user ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }
    }
}
