using Application.DTOs.StatisticDTO;
using Application.Interfaces;
using Application.Services;
using DocumentFormat.OpenXml.Vml;
using Domain.Interfaces;
using Moq;
using Persistence.Services;

namespace UnitTests.Infrastructure.Services
{
    public class ExportDataServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStatisticService> _mockStatisticService;
        private readonly ExportDataService _exportDataService;
        private readonly string _userId = Guid.NewGuid().ToString();

        public ExportDataServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStatisticService = new Mock<IStatisticService>();
            _exportDataService = new ExportDataService(_mockUnitOfWork.Object, _mockStatisticService.Object);
        }

        [Fact]
        public async Task ExportDataToCSV_ReturnsCorrectFormat()
        {
            // Arrange
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var statistics = new AllStatisticDataDTO
            {
                IncomeStatistics = new List<IncomeStatisticDTO>
                {
                    new IncomeStatisticDTO { Month = startDate, TotalAmount = 1000 },
                    new IncomeStatisticDTO { Month = endDate, TotalAmount = 1500 }
                },
                ExpensesStatistics = new List<TotalExpensesDTO>
                {
                    new TotalExpensesDTO { Month = startDate, TotalAmount = 500 },
                    new TotalExpensesDTO { Month = endDate, TotalAmount = 600 }
                },
                SavingsStatistics = new List<SavedStatisticDTO>
                {
                    new SavedStatisticDTO { Month = startDate, TotalAmount = 500 },
                    new SavedStatisticDTO { Month = endDate, TotalAmount = 900 }
                }
            };

            _mockStatisticService.Setup(s => s.GetAllData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                     .ReturnsAsync(statistics);

            // Act
            var result = await _exportDataService.ExportDataToCSV(startDate, endDate, _userId);

            // Assert
            var expectedHeader = "Month,Total Income,Total Expenses,Total Savings";
            var expectedFirstLine = $"{startDate.ToString("MMMM yyyy")},1000,500,500";
            var expectedSecondLine = $"{endDate.ToString("MMMM yyyy")},1500,600,900";
            var resultLines = result.Split('\n').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

            Assert.Equal(expectedHeader, resultLines[0]);
            Assert.Contains(expectedFirstLine, resultLines);
            Assert.Contains(expectedSecondLine, resultLines);
        }

        [Fact]
        public async Task ExportDataToXML_ReturnsCorrectFormat()
        {
            // Arrange
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var statistics = new AllStatisticDataDTO
            {
                IncomeStatistics = new List<IncomeStatisticDTO>
                {
                    new IncomeStatisticDTO { Month = startDate, TotalAmount = 1000 },
                    new IncomeStatisticDTO { Month = endDate, TotalAmount = 1500 }
                },
                ExpensesStatistics = new List<TotalExpensesDTO>
                {
                    new TotalExpensesDTO { Month = startDate, TotalAmount = 500 },
                    new TotalExpensesDTO { Month = endDate, TotalAmount = 600 }
                },
                SavingsStatistics = new List<SavedStatisticDTO>
                {
                    new SavedStatisticDTO { Month = startDate, TotalAmount = 500 },
                    new SavedStatisticDTO { Month = endDate, TotalAmount = 900 }
                }
            };

            _mockStatisticService.Setup(s => s.GetAllData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                     .ReturnsAsync(statistics);

            // Act
            var result = await _exportDataService.ExportDataToXML(startDate, endDate, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ExportDataToXSLX_ReturnsCorrectFormat()
        {
            // Arrange
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var statistics = new AllStatisticDataDTO
            {
                IncomeStatistics = new List<IncomeStatisticDTO>
                {
                    new IncomeStatisticDTO { Month = startDate, TotalAmount = 1000 },
                    new IncomeStatisticDTO { Month = endDate, TotalAmount = 1500 }
                },
                ExpensesStatistics = new List<TotalExpensesDTO>
                {
                    new TotalExpensesDTO { Month = startDate, TotalAmount = 500 },
                    new TotalExpensesDTO { Month = endDate, TotalAmount = 600 }
                },
                SavingsStatistics = new List<SavedStatisticDTO>
                {
                    new SavedStatisticDTO { Month = startDate, TotalAmount = 500 },
                    new SavedStatisticDTO { Month = endDate, TotalAmount = 900 }
                }
            };

            _mockStatisticService.Setup(s => s.GetAllData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                     .ReturnsAsync(statistics);

            // Act
            var result = await _exportDataService.ExportDataToXLSX(startDate, endDate, _userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
