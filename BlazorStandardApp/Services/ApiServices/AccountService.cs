using BlazorStandardApp.Models;
using BlazorStandardApp.Models.Commands.Users;
using BlazorStandardApp.Models.Responses.Users;
using BlazorStandardApp.Utilities;

namespace BlazorStandardApp.Services.ApiServices
{
    public interface IAccountService
    {
        Task<ModelServiceResponse<AccountResponse>> Create(AccountCommand command);
    }

    public class AccountService(HttpRequestHandler<Program> requestHandler, NotifyService notifyService)
        : BaseApiService(requestHandler, notifyService)
        , IAccountService
    {
        public async Task<ModelServiceResponse<AccountResponse>> Create(AccountCommand command)
        {
            var response = await RequestHandler
                .PostAsync<AccountResponse, AccountCommand>("api/account/create", command, CancellationToken.None);

            if (!response.IsSuccesfull)
            {
                return new ModelServiceResponse<AccountResponse>(response.StatusCode, default)
                {
                    ValidationErrors = response.ValidationErrors
                };
            }

            return response;
        }
    }
}
