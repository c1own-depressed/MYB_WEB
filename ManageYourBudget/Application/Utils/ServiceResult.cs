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
