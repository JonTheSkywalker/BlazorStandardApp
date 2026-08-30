using BlazorStandardApp.Services.Constants;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorStandardApp.Services.Authentication
{
    // Attaches the persisted JWT as a Bearer token to every outgoing API request.
    public class BearerStateHandler(CircuitServicesAccessor circuitServicesAccessor)
        : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var services = circuitServicesAccessor?.Services;

            if (services is not null)
            {
                var localStorageProvider = services.GetRequiredService<ProtectedLocalStorage>();
                var token = await localStorageProvider.GetAsync<string>(AuthConstants.Purpose, AuthConstants.AuthKey);
                if (token.Success && token.Value is not null)
                {
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", token.Value));
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
