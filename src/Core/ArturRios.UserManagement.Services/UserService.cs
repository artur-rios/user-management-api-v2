using ArturRios.Common.Extensions;
using ArturRios.Common.Output;
using ArturRios.Common.Util.Hashing;
using ArturRios.Common.Validation;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Domain.Filters;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Dto;
using ArturRios.UserManagement.Dto.Mapping;

namespace ArturRios.UserManagement.Services;

public class UserService(IUserRepository userRepository, IFluentValidator<UserDto> userValidator)
{
    public DataOutput<int> CreateAdmin(UserDto userDto)
    {
        userDto.RoleId = (int)Roles.Admin;

        return Create(userDto);
    }

    public DataOutput<int> CreateRegular(UserDto userDto)
    {
        userDto.RoleId = (int)Roles.Regular;

        return Create(userDto);
    }

    private DataOutput<int> Create(UserDto userDto)
    {
        var validationErrors = userValidator.ValidateAndReturnErrors(userDto);

        if (validationErrors.IsNotEmpty())
        {
            return new DataOutput<int>(0, validationErrors, false);
        }

        var filter = new UserFilter { Email = userDto.Email };

        var userSearch = userRepository.GetByFilter(filter);

        if (userSearch.Any())
        {
            return new DataOutput<int>(0, ["E-mail already registered"], false);
        }

        var user = userDto.ToEntity();

        var hash = Hash.NewFromText(userDto.Password);

        user.SetPassword(hash.Value, hash.Salt);

        var userId = userRepository.Create(user);

        return new DataOutput<int>(userId, ["User created with success"], true);
    }

    public DataOutput<UserDto> GetById(int id)
    {
        var user = userRepository.GetById(id);

        var message = user is null ? "User not found" : "User found";

        return new DataOutput<UserDto>(user?.ToDto(), [message], true);
    }

    public DataOutput<IList<UserDto>> GetByFilter(UserFilter filter)
    {
        var userSearch = userRepository.GetByFilter(filter).ToList();

        if (userSearch.Count == 0)
        {
            return new DataOutput<IList<UserDto>>([], ["No users found for the given filter"], true);
        }

        var users = userSearch.Select(user => user.ToDto()).ToList();

        return new DataOutput<IList<UserDto>>(users, ["Search completed with success"], true);
    }

    public DataOutput<IList<UserDto>> GetByMultiFilter(UserMultiFilter filter)
    {
        var userSearch = userRepository.GetByMultiFilter(filter).ToList();

        if (userSearch.Count == 0)
        {
            return new DataOutput<IList<UserDto>>([], ["No users found for the given filter"], true);
        }

        var users = userSearch.Select(user => user.ToDto()).ToList();

        return new DataOutput<IList<UserDto>>(users, ["Search completed with success"], true);
    }

    public ProcessOutput ChangeRole(int userId, int newRoleId)
    {
        var user = userRepository.GetById(userId);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var result = user.SetRole(newRoleId);

        if (result.Success)
        {
            userRepository.Update(user);
        }

        return result;
    }

    public DataOutput<UserDto?> Update(UserDto userDto)
    {
        var validationErrors = userValidator.ValidateAndReturnErrors(userDto);

        if (validationErrors.IsNotEmpty())
        {
            return new DataOutput<UserDto?>(null, validationErrors, false);
        }

        var user = userRepository.GetById(userDto.Id);

        if (user is null)
        {
            return new DataOutput<UserDto?>(null, ["User not found"], false);
        }

        var result = user.Update(userDto.ToEntity());

        if (result.Success)
        {
            userRepository.Update(user);
        }
        else
        {
            return new DataOutput<UserDto?>(null, result.Errors.ToArray(), false);
        }

        return new DataOutput<UserDto?>(user.ToDto(), ["User updated with success"], true);
    }

    public ProcessOutput Activate(int id)
    {
        var user = userRepository.GetById(id);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var result = user.Activate();

        if (result.Success)
        {
            userRepository.Update(user);
        }

        return result;
    }

    public ProcessOutput ActivateMany(int[] ids)
    {
        List<int> notFoundIds = [];
        List<(int, string[])> failedActivations = [];

        foreach (var id in ids)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var result = user.Activate();

            if (result.Success)
            {
                userRepository.Update(user);
            }
            else
            {
                failedActivations.Add((id, result.Errors.ToArray()));
            }
        }

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotActivateMessage = failedActivations.Count > 0
            ? $"Users with IDs {string.Join(", ", failedActivations.Select(x => x.Item1))} cannot be activated"
            : string.Empty;

        List<string> errors = [];

        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }

        if (string.IsNullOrEmpty(cannotActivateMessage))
        {
            return new ProcessOutput(errors);
        }

        errors.Add(cannotActivateMessage);

        foreach (var cannotActivate in failedActivations)
        {
            errors.Add($"User with Id {cannotActivate.Item1}: {string.Join(", ", cannotActivate.Item2)}");
        }

        return new ProcessOutput(errors);
    }

    public ProcessOutput Deactivate(int id)
    {
        var user = userRepository.GetById(id);

        if (user is null)
        {
            return new ProcessOutput(["User not found"]);
        }

        var result = user.Deactivate();

        if (result.Success)
        {
            userRepository.Update(user);
        }

        return result;
    }

    public ProcessOutput DeactivateMany(int[] ids)
    {
        List<int> notFoundIds = [];
        List<(int, string[])> failedDeactivations = [];

        foreach (var id in ids)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var result = user.Deactivate();

            if (result.Success)
            {
                userRepository.Update(user);
            }
            else
            {
                failedDeactivations.Add((id, result.Errors.ToArray()));
            }
        }

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotDeactivateMessage = failedDeactivations.Count > 0
            ? $"Users with IDs {string.Join(", ", failedDeactivations.Select(x => x.Item1))} cannot be deactivated"
            : string.Empty;

        List<string> errors = [];

        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }

        if (string.IsNullOrEmpty(cannotDeactivateMessage))
        {
            return new ProcessOutput(errors);
        }

        errors.Add(cannotDeactivateMessage);

        foreach (var cannotDeactivate in failedDeactivations)
        {
            errors.Add($"User with Id {cannotDeactivate.Item1}: {string.Join(", ", cannotDeactivate.Item2)}");
        }

        return new ProcessOutput(errors);
    }

    public ProcessOutput Delete(int id)
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

    public ProcessOutput DeleteMany(int[] ids)
    {
        List<int> idsToDelete = [];
        List<int> notFoundIds = [];
        List<(int, string[])> failedDeletions = [];

        foreach (var id in ids)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);

                continue;
            }

            var canDelete = user.CanDelete();

            if (!canDelete.Success)
            {
                failedDeletions.Add((id, canDelete.Errors.ToArray()));

                continue;
            }

            idsToDelete.Add(id);
        }

        userRepository.MultiDelete(idsToDelete);

        var notFoundMessage = notFoundIds.Count > 0
            ? $"Users with IDs {string.Join(", ", notFoundIds)} not found"
            : string.Empty;
        var cannotDeleteMessage = failedDeletions.Count > 0
            ? $"Users with IDs {string.Join(", ", failedDeletions.Select(x => x.Item1))} cannot be deleted"
            : string.Empty;

        List<string> errors = [];

        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }

        if (string.IsNullOrEmpty(cannotDeleteMessage))
        {
            return new ProcessOutput(errors);
        }

        errors.Add(cannotDeleteMessage);

        foreach (var cannotDelete in failedDeletions)
        {
            errors.Add($"User with Id {cannotDelete.Item1}: {string.Join(", ", cannotDelete.Item2)}");
        }

        return new ProcessOutput(errors);
    }
}