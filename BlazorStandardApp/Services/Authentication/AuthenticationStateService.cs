using System.Security.Claims;

namespace BlazorStandardApp.Services.Authentication
{
    public class AuthenticationStateService
    {
        private ClaimsPrincipal? currentUser;

        public event Action<ClaimsPrincipal>? UserChanged;

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new(); }
            set
            {
                currentUser = value;
                UserChanged?.Invoke(currentUser);
            }
        }
    }
}
