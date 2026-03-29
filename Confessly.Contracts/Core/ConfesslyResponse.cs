namespace Confessly.Contracts.Core
{
    public class ConfesslyResponse<T>
    {
        public bool Success { get; init; } = true;
        public string Message { get; init; } = "Request is successful.";
        public T? Data { get; init; }
        public string? TraceId { get; init; }

        public static ConfesslyResponse<T> Ok(T data, string? traceId = null)
            => new()
            {
                Success = true,
                Data = data,
                TraceId = traceId
            };

        public static ConfesslyResponse<T> Fail(string message, string? traceId = null)
            => new()
            {
                Success = false,
                Message = message,
                TraceId = traceId
            };
    }
}
