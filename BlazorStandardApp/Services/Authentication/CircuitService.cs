using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorStandardApp.Services.Authentication
{
    // Makes the current circuit's scoped IServiceProvider available to non-component
    // services (like the BearerStateHandler DelegatingHandler) via an AsyncLocal.
    public class CircuitServicesAccessor
    {
        static readonly AsyncLocal<IServiceProvider> blazorServices = new();

        public IServiceProvider? Services
        {
            get => blazorServices.Value;
            set => blazorServices.Value = value;
        }
    }

    public class ServicesAccessorCircuitHandler(
        IServiceProvider services,
        CircuitServicesAccessor servicesAccessor)
        : CircuitHandler
    {
        public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
            Func<CircuitInboundActivityContext, Task> next)
        {
            return async context =>
            {
                servicesAccessor.Services = services;
                await next(context);
                servicesAccessor.Services = null;
            };
        }
    }

    public static class CircuitServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddCircuitServicesAccessor(
            this IServiceCollection services)
        {
            services.AddScoped<CircuitServicesAccessor>();
            services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();

            return services;
        }
    }
}
