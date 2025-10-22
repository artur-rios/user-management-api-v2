using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class ActivateUserCommandValidator : FluentValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator() => RuleFor(command => command.UserId).GreaterThan(0);
}
