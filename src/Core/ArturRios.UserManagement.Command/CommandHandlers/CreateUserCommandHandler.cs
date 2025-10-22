using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Util.Hashing;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class CreateUserCommandHandler(IFluentValidator<CreateUserCommand> validator, IUserRepository userRepository)
    : ICommandHandler<CreateUserCommand, CreateUserCommandOutput>
{
    public DataOutput<CreateUserCommandOutput?> Handle(CreateUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<CreateUserCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        var existingUser = userRepository.GetAll().FirstOrDefault(u => u.Email == command.Email);

        if (existingUser is not null)
        {
            output.AddError("E-mail already registered");

            return output;
        }

        var user = new User(command.Email, command.Name, command.RoleId);

        var hash = Hash.NewFromText(command.Password);

        user.SetPassword(hash.Value, hash.Salt);

        var id = userRepository.Create(user);

        var data = new CreateUserCommandOutput { CreatedUserId = id };

        output.Data = data;
        output.AddMessage("User created successfully");

        return output;
    }
}
