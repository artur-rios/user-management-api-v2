using Microsoft.Extensions.Options;
using TechCraftsmen.Core.Util;

namespace TechCraftsmen.Management.User.WebApi.DependencyInjection;

public static class AuthenticationConfiguration
{
    public static void AddJwtTokenConfiguration(this IServiceCollection services, IConfigurationSection jwtTokenConfiguration)
    {
        services.AddSingleton<JwtTokenConfiguration>(_ => new JwtTokenConfiguration(
            Audience: Environment.GetEnvironmentVariable("AUTH_TOKEN_AUDIENCE")!,
            ExpirationInSeconds: double.TryParse(Environment.GetEnvironmentVariable("AUTH_TOKEN_EXPIRATION_IN_SECONDS"), out var seconds)
                ? seconds
                : 3600,
            Issuer: Environment.GetEnvironmentVariable("AUTH_TOKEN_ISSUER")!,
            Secret: Environment.GetEnvironmentVariable("AUTH_TOKEN_SECRET")!
        ));

        services.AddSingleton<IOptions<JwtTokenConfiguration>>(provider =>
            new OptionsWrapper<JwtTokenConfiguration>(provider.GetRequiredService<JwtTokenConfiguration>()));
    }
}
