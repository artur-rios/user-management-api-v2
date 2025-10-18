using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Domain.Enums;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class UpdateUserRoleCommandValidator : FluentValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.NewRoleId).Custom((roleId, context) =>
        {
            var validRole = Enum.IsDefined(typeof(Roles), roleId);

            if (!validRole)
            {
                context.AddFailure("Role should be valid");
            }
        });
    }
}
