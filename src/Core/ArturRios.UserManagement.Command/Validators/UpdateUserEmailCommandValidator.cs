using ArturRios.Common.Extensions;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Command.Commands;
using FluentValidation;

namespace ArturRios.UserManagement.Command.Validators;

public class UpdateUserEmailCommandValidator : FluentValidator<UpdateUserEmailCommand>
{
    public UpdateUserEmailCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(user => user.Email).Custom((email, context) =>
        {
            if (!email.IsValidEmail())
            {
                context.AddFailure("Email should be valid");
            }
        });
    }
}
