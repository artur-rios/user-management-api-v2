using TechCraftsmen.Core.Validation;

namespace TechCraftsmen.Management.User.WebApi.DependencyInjection;

public static class AuthenticationConfiguration
{
    public static void AddJwtTokenConfiguration(this IServiceCollection services, IConfigurationSection jwtTokenConfiguration)
    {
        services.Configure<JwtTokenConfiguration>(jwtTokenConfiguration);
    }
}