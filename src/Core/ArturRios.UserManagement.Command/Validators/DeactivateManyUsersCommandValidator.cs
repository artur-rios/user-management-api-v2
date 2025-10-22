using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class DeactivateManyUsersCommandValidator : FluentValidator<DeactivateManyUsersCommand>
{
    public DeactivateManyUsersCommandValidator()
    {
        RuleFor(command => command.UserIds).NotEmpty();
        RuleForEach(command => command.UserIds).GreaterThan(0);
    }
}
