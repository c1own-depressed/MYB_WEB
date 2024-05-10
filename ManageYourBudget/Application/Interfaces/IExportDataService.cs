using Application.Utils;

namespace Application.Interfaces
{
    public interface IExportDataService
    {
        Task<string> ExportDataToCSV(DateTime startDate, DateTime endDate, string userId);

        Task<byte[]> ExportDataToXML(DateTime startDate, DateTime endDate, string userId);

        Task<byte[]> ExportDataToXLSX(DateTime startDate, DateTime endDate, string userId);

        Task<FileExportResult> ExportData(DateTime startDate, DateTime endDate, string userId, ExportFormat format);
    }
}
