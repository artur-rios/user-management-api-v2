using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class UpdateUserCommandHandler(IFluentValidator<UpdateUserCommand> validator, IUserRepository userRepository) : ICommandHandler<UpdateUserCommand, UpdateUserCommandOutput>
{
    public DataOutput<UpdateUserCommandOutput?> Handle(UpdateUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<UpdateUserCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        var user = userRepository.GetById(command.Id);

        if (user is null)
        {
            output.AddError("User not found");

            return output;
        }

        var result = user.Update(command.Name);

        if (result.Success)
        {
            userRepository.Update(user);

            var data = new UpdateUserCommandOutput
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                CreatedAt = user.CreatedAt,
                Active = user.Active
            };

            output.Data = data;
            output.AddMessage("User updated with success");
        }
        else
        {
            output.AddErrors(result.Errors);
        }

        return output;
    }
}
