namespace ApiGenerico.WebAPI.Models
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
