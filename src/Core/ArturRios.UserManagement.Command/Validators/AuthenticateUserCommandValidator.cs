using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class AuthenticateUserCommandValidator : FluentValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().WithMessage("Email should be valid");
        RuleFor(command => command.Password).NotEmpty().WithMessage("Password should be valid");
    }
}
