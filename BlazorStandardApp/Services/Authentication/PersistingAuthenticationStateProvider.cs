using BlazorStandardApp.Models;
using BlazorStandardApp.Services.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorStandardApp.Services.Authentication
{
    // Keeps the signed-in user in sync with a JWT that is persisted in the browser's
    // protected local storage, so a login survives page reloads and reconnects.
    public class PersistingAuthenticationStateProvider
        : AuthenticationStateProvider
        , IDisposable
    {
        private AuthenticationState _authenticationState;
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly AuthenticationStateService _service;

        public PersistingAuthenticationStateProvider(
            AuthenticationStateService service,
            ProtectedLocalStorage protectedLocalStorage)
        {
            _service = service;
            _authenticationState = new AuthenticationState(service.CurrentUser);
            _service.UserChanged += Service_UserChanged;
            _protectedLocalStorage = protectedLocalStorage;
        }

        private void Service_UserChanged(ClaimsPrincipal newUser)
        {
            _authenticationState = new AuthenticationState(newUser);
            NotifyAuthenticationStateChanged(Task.FromResult(_authenticationState));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _service.UserChanged -= Service_UserChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var storageResult = await _protectedLocalStorage
                    .GetAsync<string>(AuthConstants.Purpose, AuthConstants.AuthKey)
                    .ConfigureAwait(false);

                if (storageResult.Success && !string.IsNullOrWhiteSpace(storageResult.Value))
                {
                    _service.CurrentUser = BuildPrincipal(storageResult.Value);
                }
            }
            catch
            {
                // JS interop (protected storage) is unavailable during prerendering.
                // Treat as anonymous; the real state loads once the circuit is interactive.
            }

            return _authenticationState;
        }

        public async Task<AuthenticationResult> SignIn(string token)
        {
            try
            {
                var principal = BuildPrincipal(token);
                await _protectedLocalStorage
                    .SetAsync(AuthConstants.Purpose, AuthConstants.AuthKey, token)
                    .ConfigureAwait(false);
                _service.CurrentUser = principal;
                return new AuthenticationResult();
            }
            catch (Exception)
            {
                return new AuthenticationResult(AuthConstants.AuthenticationException);
            }
        }

        public async Task SignOut()
        {
            await _protectedLocalStorage.DeleteAsync(AuthConstants.AuthKey);
            _service.CurrentUser = new ClaimsPrincipal();
        }

        private static ClaimsPrincipal BuildPrincipal(string token)
        {
            var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(securityToken.Claims, AuthConstants.AuthenticationType);
            return new ClaimsPrincipal(identity);
        }
    }
}
