using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Commands.Interfaces;
using ArturRios.Common.Util.Hashing;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Repositories;

namespace ArturRios.UserManagement.Command.CommandHandlers;

public class AuthenticateUserCommandHandler(
    IFluentValidator<AuthenticateUserCommand> validator,
    IUserReadOnlyRepository userRepository) : ICommandHandler<AuthenticateUserCommand, AuthenticateUserCommandOutput>
{
    public DataOutput<AuthenticateUserCommandOutput?> Handle(AuthenticateUserCommand command)
    {
        var validationResult = validator.Validate(command);

        var output = new DataOutput<AuthenticateUserCommandOutput?>();

        if (!validationResult.IsValid)
        {
            output.AddErrors(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

            return output;
        }

        var user = userRepository.GetAll().FirstOrDefault(u => u.Email == command.Email);

        if (user is null)
        {
            output.AddError("Invalid credentials");

            return output;
        }

        var passwordHash = Hash.NewFromBytes(value: user.Password, salt: user.Salt);

        if (!passwordHash.TextMatches(command.Password))
        {
            output.AddError("Invalid credentials");

            return output;
        }

        var data = new AuthenticateUserCommandOutput { Id = user.Id, RoleId = user.RoleId };

        output.Data = data;
        output.AddMessage("User authenticated successfully");

        return output;
    }
}
