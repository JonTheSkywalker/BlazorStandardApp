namespace BlazorStandardApp.Models
{
    public class AuthenticationResult(string? errorCode = null)
    {
        public bool IsAuthenticated => string.IsNullOrWhiteSpace(ErrorCode);
        public string? ErrorCode { get; internal set; } = errorCode;
    }
}
