using BlazorStandardApp.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorStandardApp.Components.Pages.Auth
{
    public partial class Logout
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

        private bool _hasLoggedOut = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_hasLoggedOut)
            {
                _hasLoggedOut = true;
                await ((PersistingAuthenticationStateProvider)AuthStateProvider).SignOut();
                NavigationManager.NavigateTo("/login");
            }
        }
    }
}
