using FluentValidation;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Dto.Validation;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    private const int MinimumPasswordLength = 8;
    
    public UserDtoValidator()
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
                          && password.Length >= MinimumPasswordLength;
            
            if (!isValid)
            {
                context.AddFailure($"Password must contain at least {MinimumPasswordLength} characters, a number, a lower char and an upper char");
            }
        });
        RuleFor(user => user.RoleId).Custom((roleId, context) =>
        {
            var validRole = Enum.GetValues(typeof(Roles)).Cast<Roles>().Any(role => roleId == (int)role);
            
            if (!validRole)
            {
                context.AddFailure("Role should be valid");
            }
        });
    }
}