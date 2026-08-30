using BlazorStandardApp.Services;
using BlazorStandardApp.Services.ApiServices;
using BlazorStandardApp.Services.Authentication;
using BlazorStandardApp.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlazorStandardApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationAndAuthorizationServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCircuitServicesAccessor()
                .AddCascadingAuthenticationState()
                .AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateService>()
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    var signingKey = configuration["Authentication:Bearer:SigningKey"];
                    SecurityKey? key = null;
                    if (!string.IsNullOrWhiteSpace(signingKey))
                    {
                        key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey));
                    }
                    o.TokenValidationParameters.IssuerSigningKey = key;
                    o.TokenValidationParameters.ValidateIssuerSigningKey = key is not null;
                    o.TokenValidationParameters.ValidateLifetime = true;
                    o.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(60);
                    o.TokenValidationParameters.ValidateAudience = false;
                    o.TokenValidationParameters.ValidateIssuer = false;
                    o.MapInboundClaims = false;
                });

            return services;
        }

        public static IServiceCollection AddApplicationSevice(this IServiceCollection services)
        {
            services
                .AddScoped<NotifyService>()
                .AddTransient<ILoginService, LoginService>()
                .AddTransient<IAccountService, AccountService>();

            return services;
        }

        public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<BearerStateHandler>()
                .AddTransient<HttpRequestHandler<Program>>()
                .AddHttpClient(typeof(Program).AssemblyQualifiedName!, client =>
                {
                    client.BaseAddress = new Uri(configuration["Api:BaseUrl"]
                        ?? throw new InvalidOperationException("Api:BaseUrl is not configured."));
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .AddHttpMessageHandler<BearerStateHandler>();

            return services;
        }
    }
}
