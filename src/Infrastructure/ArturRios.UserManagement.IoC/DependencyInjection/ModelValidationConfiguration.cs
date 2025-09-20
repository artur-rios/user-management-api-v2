using ArturRios.Common.Security;
using ArturRios.Common.Validation;
using ArturRios.Common.WebApi.Security.Records;
using ArturRios.Common.WebApi.Security.Validation;
using ArturRios.UserManagement.Dto;
using ArturRios.UserManagement.Dto.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class ModelValidationConfiguration
{
    public static void AddModelValidators(this IServiceCollection services)
    {
        services.AddScoped<IFluentValidator<Credentials>, CredentialsValidator>();
        services.AddScoped<IFluentValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<IFluentValidator<JwtTokenConfiguration>, JwtTokenConfigurationValidator>();
    }
}
