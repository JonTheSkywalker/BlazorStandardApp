namespace BlazorStandardApp.Services
{
    public class NotifyService
    {
        public event Action? ForceLogout;

        public void InvokeForceLogout() => ForceLogout?.Invoke();
    }
}
