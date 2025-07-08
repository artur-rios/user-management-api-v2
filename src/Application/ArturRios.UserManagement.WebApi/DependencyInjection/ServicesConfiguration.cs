using ArturRios.Common.WebApi.Security.Interfaces;
using ArturRios.UserManagement.Services;

namespace ArturRios.UserManagement.WebApi.DependencyInjection;

public static class ServicesConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<UserService>();
    }
}
