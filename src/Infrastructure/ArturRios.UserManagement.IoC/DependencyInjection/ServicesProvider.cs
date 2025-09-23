using ArturRios.Common.WebApi.Security.Interfaces;
using ArturRios.UserManagement.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class ServicesProvider
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<UserService>();
    }
}
