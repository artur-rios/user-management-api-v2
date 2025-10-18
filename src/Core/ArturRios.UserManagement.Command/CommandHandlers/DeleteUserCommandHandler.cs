using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class DeleteUserCommandHandler(
    IFluentValidator<DeleteUserCommand> validator,
    IUserRepository userRepository) : ICommandHandler<DeleteUserCommand, DeleteUserCommandOutput>
{
    public DataOutput<DeleteUserCommandOutput?> Handle(DeleteUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<DeleteUserCommandOutput?>();

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

        var canDelete = user.CanDelete();

        if (canDelete.Success)
        {
            userRepository.Delete(user);
        }
        else
        {
            output.AddErrors(canDelete.Errors);

            return output;
        }

        output.AddMessage("User deleted successfully");

        return output;
    }
}
