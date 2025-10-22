using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class DeactivateUserCommandValidator : FluentValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator() => RuleFor(command => command.UserId).GreaterThan(0);
}
