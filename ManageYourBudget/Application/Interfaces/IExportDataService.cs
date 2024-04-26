using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IExportDataService
    {
        Task<string> ExportDataToCSV(DateTime startDate, DateTime endDate, string userId);

        Task<byte[]> ExportDataToXML(DateTime startDate, DateTime endDate, string userId);

        Task<byte[]> ExportDataToXLSX(DateTime startDate, DateTime endDate, string userId);
    }
}
