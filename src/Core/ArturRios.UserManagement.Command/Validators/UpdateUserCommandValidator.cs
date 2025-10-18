using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class UpdateUserCommandValidator : FluentValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
    }
}
