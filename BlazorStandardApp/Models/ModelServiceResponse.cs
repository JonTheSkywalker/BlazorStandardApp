namespace BlazorStandardApp.Models
{
    public class ModelServiceResponse<T>(int statusCode, T? model)
        : ServiceResponse(statusCode)
    {
        public T? Model { get; internal set; } = model;
    }

    public class ServiceResponse(int statusCode)
    {
        public int StatusCode { get; internal set; } = statusCode;
        public bool IsSuccesfull => StatusCode >= 200 && StatusCode < 300;
        public bool ShouldRefresh => StatusCode == 409;
        public bool HasValidationErrors => StatusCode == 400;
        public Dictionary<string, string[]> ValidationErrors { get; set; } = [];
    }
}
