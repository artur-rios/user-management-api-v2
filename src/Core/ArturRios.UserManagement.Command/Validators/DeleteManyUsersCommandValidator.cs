using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class DeleteManyUsersCommandValidator : FluentValidator<DeleteManyUsersCommand>
{
    public DeleteManyUsersCommandValidator()
    {
        RuleFor(command => command.UserIds).NotEmpty();
        RuleForEach(command => command.UserIds).GreaterThan(0);
    }
}
