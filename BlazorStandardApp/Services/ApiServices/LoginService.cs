using BlazorStandardApp.Models;
using BlazorStandardApp.Models.Commands.Users;
using BlazorStandardApp.Models.Responses.Users;
using BlazorStandardApp.Utilities;

namespace BlazorStandardApp.Services.ApiServices
{
    public interface ILoginService
    {
        Task<ModelServiceResponse<LoginResponse>> Login(LoginCommand command);
    }

    public class LoginService(HttpRequestHandler<Program> requestHandler, NotifyService notifyService)
        : BaseApiService(requestHandler, notifyService)
        , ILoginService
    {
        public async Task<ModelServiceResponse<LoginResponse>> Login(LoginCommand command)
        {
            return await RequestHandler
                .PostAsync<LoginResponse, LoginCommand>("api/account/login", command, CancellationToken.None);
        }
    }
}
