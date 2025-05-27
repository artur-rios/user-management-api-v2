using FluentValidation;
using Microsoft.AspNetCore.Http;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.Util.Hashing;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Dto.Mapping;

namespace TechCraftsmen.Management.User.Services;

public class UserService(
    ICrudRepository<Domain.Aggregates.User> userRepository,
    IHttpContextAccessor httpContextAccessor,
    IValidator<UserDto> userValidator)
{
    public DataOutput<int> CreateUser(UserDto userDto)
    {
        var validationResult = userValidator.Validate(userDto);

        if (!validationResult.IsValid)
        {
            return validationResult.ToDataOutput<int>();
        }

        var filter = new UserFilter { Email = userDto.Email };

        var userSearch = userRepository.GetByFilter(filter);

        if (userSearch.Any())
        {
            return new DataOutput<int>(0, ["E-mail already registered"], false);
        }

        var user = userDto.ToEntity();

        httpContextAccessor.HttpContext!.Items.TryGetValue("User", out var userData);

        var authenticatedUser = userData as AuthenticatedUser;

        var canRegister = user.CanRegister(authenticatedUser!.Role);

        if (!canRegister.Success)
        {
            return new DataOutput<int>(0, canRegister.Errors.ToArray(), false);
        }

        var hash = Hash.NewFromText(userDto.Password);

        user.Password = hash.Value;
        user.Salt = hash.Salt;

        var userId = userRepository.Create(user);

        return new DataOutput<int>(userId, ["User created with success"], true);
    }

    public DataOutput<UserDto> GetUserById(int id)
    {
        var user = userRepository.GetById(id);

        var message = user is null ? "User not found" : "User found";

        return new DataOutput<UserDto>(user?.ToDto(), [message], user is not null);
    }

    public DataOutput<IList<UserDto>> GetUsersByFilter(UserFilter filter)
    {
        var userSearch = userRepository.GetByFilter(filter).ToList();

        if (userSearch.Count == 0)
        {
            return new DataOutput<IList<UserDto>>([], ["No users found for the given filter"], false);
        }

        var users = userSearch.Select(user => user.ToDto()).ToList();

        return new DataOutput<IList<UserDto>>(users, ["Users found"], true);
    }

    public DataOutput<UserDto?> UpdateUser(UserDto userDto)
    {
        var currentUser = userRepository.GetById(userDto.Id);

        if (currentUser is null)
        {
            return new DataOutput<UserDto?>(null, ["User not found"], false);
        }

        var canUpdate = currentUser.CanUpdate();

        if (!canUpdate.Success)
        {
            return new DataOutput<UserDto?>(null, canUpdate.Errors.ToArray(), false);
        }

        var user = userDto.ToEntity();

        MergeUser(currentUser, user);

        userRepository.Update(user);

        return new DataOutput<UserDto?>(user.ToDto(), ["User updated with success"], true);
    }

    public ProcessOutput ActivateUser(int id)
    {
        var user = userRepository.GetById(id);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var canActivate = user.CanActivate();

        if (!canActivate.Success)
        {
            return new ProcessOutput(canActivate.Errors.ToArray());
        }

        user.Active = true;
        userRepository.Update(user);

        return new ProcessOutput();
    }

    public ProcessOutput DeactivateUser(int id)
    {
        var user = userRepository.GetById(id);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var canDeactivate = user.CanDeactivate();

        if (!canDeactivate.Success)
        {
            return new ProcessOutput(canDeactivate.Errors.ToArray());
        }

        user.Active = false;
        userRepository.Update(user);

        return new ProcessOutput();
    }

    public ProcessOutput DeleteUser(int id)
    {
        var user = userRepository.GetById(id);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var canDelete = user.CanDelete();

        if (!canDelete.Success)
        {
            return new ProcessOutput(canDelete.Errors.ToArray());
        }

        userRepository.Delete(id);

        return new ProcessOutput();
    }

    private static void MergeUser(Domain.Aggregates.User source, Domain.Aggregates.User target, bool mergeStatus = true)
    {
        target.Password = source.Password;
        target.Salt = source.Salt;
        target.CreatedAt = source.CreatedAt;

        if (mergeStatus)
        {
            target.Active = source.Active;
        }
    }
}
