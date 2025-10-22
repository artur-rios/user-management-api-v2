using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class DeleteUserCommandValidator : FluentValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator() => RuleFor(command => command.UserId).GreaterThan(0);
}
