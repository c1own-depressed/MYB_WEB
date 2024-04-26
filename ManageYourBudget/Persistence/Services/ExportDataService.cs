using System.Text;
using System.Xml.Linq;
using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Application.Services;
using ClosedXML.Excel;
using Domain.Interfaces;

namespace Persistence.Services
{
    public class ExportDataService : IExportDataService
    {
        private IUnitOfWork _unitOfWork;
        private IStatisticService _statisticService;

        public ExportDataService(IUnitOfWork unitOfWork, IStatisticService statisticService)
        {
            _unitOfWork = unitOfWork;
            _statisticService = statisticService;
        }

        public async Task<string> ExportDataToCSV(DateTime startDate, DateTime endDate, string userId)
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

            return stringBuilder.ToString();
        }

        public async Task<byte[]> ExportDataToXML(DateTime startDate, DateTime endDate, string userId)
        {
            var data = await _statisticService.GetAllData(startDate, endDate, userId);
            var xDocument = new XDocument(new XElement(
                "Statistics",
                new XElement(
                    "Incomes",
                    data.IncomeStatistics.Select(income => new XElement(
                         "Income",
                         new XElement("Month", income.Month.ToString("MMMM yyyy")),
                         new XElement("TotalAmount", income.TotalAmount)))),
                new XElement(
                    "Expenses",
                    data.ExpensesStatistics.Select(expense => new XElement(
                        "Expense",
                        new XElement("Month", expense.Month.ToString("MMMM yyyy")),
                        new XElement("TotalAmount", expense.TotalAmount)))),
                new XElement("Savings",
                    data.SavingsStatistics.Select(saving => new XElement(
                        "Saving",
                        new XElement("Month", saving.Month.ToString("MMMM yyyy")),
                        new XElement("TotalAmount", saving.TotalAmount))))));

            using (var memoryStream = new MemoryStream())
            {
                xDocument.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportDataToXLSX(DateTime startDate, DateTime endDate, string userId)
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
    }
}
