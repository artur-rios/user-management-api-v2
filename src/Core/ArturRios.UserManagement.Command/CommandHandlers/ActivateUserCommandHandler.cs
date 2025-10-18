using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class ActivateUserCommandHandler(IFluentValidator<ActivateUserCommand> validator, IUserRepository userRepository) : ICommandHandler<ActivateUserCommand, ActivateUserCommandOutput>
{
    public DataOutput<ActivateUserCommandOutput?> Handle(ActivateUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<ActivateUserCommandOutput?>();

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

        var result = user.Activate();

        if (result.Success)
        {
            userRepository.Update(user);
        }
        else
        {
            output.AddErrors(result.Errors);

            return output;
        }

        output.AddMessage("User activated successfully");

        return output;
    }
}
