using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class UpdateUserEmailCommandHandler(
    IFluentValidator<UpdateUserEmailCommand> validator,
    IUserRepository userRepository) : ICommandHandler<UpdateUserEmailCommand, UpdateUserEmailCommandOutput>
{
    public DataOutput<UpdateUserEmailCommandOutput?> Handle(UpdateUserEmailCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<UpdateUserEmailCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        var user = userRepository.GetAll().FirstOrDefault(user => user.Email == command.Email);

        if (user is null)
        {
            output.AddError("User not found");

            return output;
        }


        if (user.Id != command.UserId)
        {
            output.AddError("Email is already in use");

            return output;
        }

        var result = user.UpdateEmail(command.Email);

        if (result.Success)
        {
            userRepository.Update(user);
        }
        else
        {
            output.AddErrors(result.Errors);

            return output;
        }

        var data = new UpdateUserEmailCommandOutput { UpdatedEmail = command.Email };

        output.AddMessage($"User e-mail updated to {command.Email}");
        output.Data = data;

        return output;
    }
}
