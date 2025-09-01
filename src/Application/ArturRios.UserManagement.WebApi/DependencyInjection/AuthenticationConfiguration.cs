using ArturRios.Common.Security;
using Microsoft.Extensions.Options;

namespace ArturRios.UserManagement.WebApi.DependencyInjection;

public static class AuthenticationConfiguration
{
    public static void AddJwtTokenConfiguration(this IServiceCollection services)
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
