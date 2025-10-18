using ArturRios.Common.Security;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class CommandValidationProvider
{
    public static void AddCommandValidators(this IServiceCollection services)
    {
        services.AddScoped<IFluentValidator<ActivateManyUsersCommand>, ActivateManyUsersCommandValidator>();
        services.AddScoped<IFluentValidator<ActivateUserCommand>, ActivateUserCommandValidator>();
        services.AddScoped<IFluentValidator<AuthenticateUserCommand>, AuthenticateUserCommandValidator>();
        services.AddScoped<IFluentValidator<CreateUserCommand>, CreateUserCommandValidator>();
        services.AddScoped<IFluentValidator<DeactivateManyUsersCommand>, DeactivateManyUsersCommandValidator>();
        services.AddScoped<IFluentValidator<DeactivateUserCommand>, DeactivateUserCommandValidator>();
        services.AddScoped<IFluentValidator<DeleteManyUsersCommand>, DeleteManyUsersCommandValidator>();
        services.AddScoped<IFluentValidator<DeleteUserCommand>, DeleteUserCommandValidator>();
        services.AddScoped<IFluentValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
        services.AddScoped<IFluentValidator<UpdateUserEmailCommand>, UpdateUserEmailCommandValidator>();
        services.AddScoped<IFluentValidator<UpdateUserRoleCommand>, UpdateUserRoleCommandValidator>();
    }
}
