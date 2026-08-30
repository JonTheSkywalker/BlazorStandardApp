using BlazorStandardApp.Models.Commands.Users;
using BlazorStandardApp.Services.ApiServices;
using BlazorStandardApp.Services.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorStandardApp.Components.Pages.Auth
{
    public partial class Login
    {
        private readonly LoginCommand _loginCommand = new();
        private readonly AccountCommand _registerCommand = new();
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private bool _success = false;
        private string mode = "login";

        private async Task HandleLogin()
        {
            _errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(_loginCommand.Email) || string.IsNullOrWhiteSpace(_loginCommand.Password))
            {
                _errorMessage = "Vul je e-mailadres en wachtwoord in.";
                return;
            }

            _isLoading = true;

            try
            {
                var result = await LoginService.Login(_loginCommand);

                if (result.IsSuccesfull && result.Model is not null && !string.IsNullOrWhiteSpace(result.Model.Token))
                {
                    var authProvider = (PersistingAuthenticationStateProvider)AuthStateProvider;
                    var authResult = await authProvider.SignIn(result.Model.Token);

                    if (authResult.IsAuthenticated)
                    {
                        Navigation.NavigateTo("/", forceLoad: false);
                        return;
                    }
                }

                _errorMessage = result.StatusCode == 401
                    ? "Ongeldige e-mail of wachtwoord."
                    : "Er is iets misgegaan. Probeer het later opnieuw.";
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleLoginKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await HandleLogin();
            }
        }

        private async Task HandleRegister()
        {
            _errorMessage = string.Empty;
            _success = false;

            if (string.IsNullOrWhiteSpace(_registerCommand.Username) ||
                string.IsNullOrWhiteSpace(_registerCommand.Email) ||
                string.IsNullOrWhiteSpace(_registerCommand.Password))
            {
                _errorMessage = "Vul alle velden in.";
                return;
            }

            if (_registerCommand.Password.Length < 8)
            {
                _errorMessage = "Je wachtwoord moet minimaal 8 tekens bevatten.";
                return;
            }

            _isLoading = true;

            try
            {
                var result = await AccountService.Create(_registerCommand);

                if (!result.IsSuccesfull)
                {
                    _errorMessage = result.StatusCode == 400
                        ? "Dit e-mailadres is al in gebruik."
                        : "Er is een fout opgetreden. Probeer het later opnieuw.";
                    return;
                }

                _success = true;
                // Pre-fill the login form so the user can sign in straight away.
                _loginCommand.Email = _registerCommand.Email;
                _registerCommand.Password = string.Empty;
                mode = "login";
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SwitchToLogin()
        {
            mode = "login";
            _errorMessage = string.Empty;
            _registerCommand.Password = string.Empty;
        }

        private void SwitchToRegister()
        {
            mode = "register";
            _errorMessage = string.Empty;
            _success = false;
        }
    }
}
