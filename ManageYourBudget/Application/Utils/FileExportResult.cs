using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class FileExportResult
    {
        public bool Success { get; }

        public byte[] FileContents { get; }

        public string ContentType { get; }

        public string FileName { get; }

        public FileExportResult(bool success, byte[] fileContents = null, string contentType = null, string fileName = null)
        {
            Success = success;
            FileContents = fileContents;
            ContentType = contentType;
            FileName = fileName;
        }
    }
}
