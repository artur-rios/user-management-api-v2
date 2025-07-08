using ArturRios.Common.Security;
using ArturRios.Common.WebApi.Security.Records;
using ArturRios.Common.WebApi.Security.Validation;
using ArturRios.UserManagement.Dto;
using ArturRios.UserManagement.Dto.Validation;
using FluentValidation;

namespace ArturRios.UserManagement.WebApi.DependencyInjection;

public static class ModelValidationConfiguration
{
    public static void AddModelValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Credentials>, CredentialsValidator>();
        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<IValidator<JwtTokenConfiguration>, JwtTokenConfigurationValidator>();
    }
}
