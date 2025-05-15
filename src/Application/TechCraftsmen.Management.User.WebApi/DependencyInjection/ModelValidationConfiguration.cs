using FluentValidation;
using TechCraftsmen.Core.Validation;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Core.WebApi.Security.Validation;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Dto.Validation;

namespace TechCraftsmen.Management.User.WebApi.DependencyInjection;

public static class ModelValidationConfiguration
{
    public static void AddModelValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<Credentials>, CredentialsValidator>();
        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<IValidator<JwtTokenConfiguration>, JwtTokenConfigurationValidator>();
    }
}
