using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class ServiceResult
    {
        public bool Success { get; }
        public IEnumerable<string> Errors { get; }

        public ServiceResult(bool success, IEnumerable<string>? errors = null)
        {
            Success = success;
            Errors = errors ?? Enumerable.Empty<string>();
        }
    }
}
