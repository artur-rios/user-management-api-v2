using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class DeactivateUserCommandHandler(
    IFluentValidator<DeactivateUserCommand> validator,
    IUserRepository userRepository) : ICommandHandler<DeactivateUserCommand, DeactivateUserCommandOutput>
{
    public DataOutput<DeactivateUserCommandOutput?> Handle(DeactivateUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<DeactivateUserCommandOutput?>();

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

        var result = user.Deactivate();

        if (result.Success)
        {
            userRepository.Update(user);
        }
        else
        {
            output.AddErrors(result.Errors);

            return output;
        }

        output.AddMessage("User deactivated successfully");

        return output;
    }
}
