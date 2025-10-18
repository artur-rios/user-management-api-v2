using ArturRios.Common.Extensions;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Domain;
using ArturRios.UserManagement.Domain.Enums;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class CreateUserCommandValidator : FluentValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(user => user.Name).NotEmpty();
        RuleFor(user => user.Email).Custom((email, context) =>
        {
            if (!email.IsValidEmail())
            {
                context.AddFailure("Email should be valid");
            }
        });
        RuleFor(user => user.Password).Custom((password, context) =>
        {
            var isValid = password.HasNumber()
                          && password.HasLowerChar()
                          && password.HasUpperChar()
                          && password.Length >= Constants.MinimumPasswordLength;

            if (!isValid)
            {
                context.AddFailure($"Password must contain at least {Constants.MinimumPasswordLength} characters, a number, a lower char and an upper char");
            }
        });
        RuleFor(user => user.RoleId).Custom((roleId, context) =>
        {
            var validRole = Enum.IsDefined(typeof(Roles), roleId);

            if (!validRole)
            {
                context.AddFailure("Role should be valid");
            }
        });
    }
}
