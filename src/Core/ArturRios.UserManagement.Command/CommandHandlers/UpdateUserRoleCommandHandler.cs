using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class UpdateUserRoleCommandHandler(IFluentValidator<UpdateUserRoleCommand> validator, IUserRepository userRepository) : ICommandHandler<UpdateUserRoleCommand, UpdateUserRoleCommandOutput>
{
    public DataOutput<UpdateUserRoleCommandOutput?> Handle(UpdateUserRoleCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<UpdateUserRoleCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        var user = userRepository.GetById(command.UserId);

        if (user is null)
        {
            output.AddError("User not found");

            return output;
        }

        var result = user.UpdateRole(command.NewRoleId);

        if (result.Success)
        {
            userRepository.Update(user);
        }
        else
        {
            output.AddErrors(result.Errors);

            return output;
        }

        var data = new UpdateUserRoleCommandOutput { UpdatedUserRoleId = command.NewRoleId };

        output.AddMessage($"User role updated to {command.NewRoleId}");
        output.Data = data;

        return output;
    }
}
