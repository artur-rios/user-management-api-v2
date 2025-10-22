using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class CommandProvider
{
    public static void AddCommands(this IServiceCollection services)
    {
        services
            .AddScoped<ICommandHandler<ActivateManyUsersCommand, ActivateManyUsersCommandOutput>,
                ActivateManyUsersCommandHandler>();
        services
            .AddScoped<ICommandHandler<ActivateUserCommand, ActivateUserCommandOutput>, ActivateUserCommandHandler>();
        services
            .AddScoped<ICommandHandler<AuthenticateUserCommand, AuthenticateUserCommandOutput>,
                AuthenticateUserCommandHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand, CreateUserCommandOutput>, CreateUserCommandHandler>();
        services
            .AddScoped<ICommandHandler<DeactivateManyUsersCommand, DeactivateManyUsersCommandOutput>,
                DeactivateManyUsersCommandHandler>();
        services
            .AddScoped<ICommandHandler<DeactivateUserCommand, DeactivateUserCommandOutput>,
                DeactivateUserCommandHandler>();
        services
            .AddScoped<ICommandHandler<DeleteManyUsersCommand, DeleteManyUsersCommandOutput>,
                DeleteManyUsersCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand, DeleteUserCommandOutput>, DeleteUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UpdateUserCommandOutput>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserEmailCommand, UpdateUserEmailCommandOutput>>();
        services
            .AddScoped<ICommandHandler<UpdateUserRoleCommand, UpdateUserRoleCommandOutput>,
                UpdateUserRoleCommandHandler>();
    }
}
