using BlazorStandardApp.Utilities;

namespace BlazorStandardApp.Services
{
    public abstract class BaseApiService(HttpRequestHandler<Program> requestHandler, NotifyService notifyService)
    {
        public HttpRequestHandler<Program> RequestHandler { get; } = requestHandler;
        public NotifyService NotifyService { get; } = notifyService;
    }
}
