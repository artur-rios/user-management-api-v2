using FluentValidation;
using Microsoft.AspNetCore.Http;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.Util.Hashing;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Domain.Interfaces;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Dto.Mapping;

namespace TechCraftsmen.Management.User.Services;

public class UserService(
    IUserRepository userRepository,
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
            return new DataOutput<IList<UserDto>>([], ["No users found for the given filter"], true);
        }

        var users = userSearch.Select(user => user.ToDto()).ToList();

        return new DataOutput<IList<UserDto>>(users, ["Search completed with success"], true);
    }

    public DataOutput<IList<UserDto>> GetUsersByMultiFilter(UserMultiFilter filter)
    {
        var userSearch = userRepository.GetByMultiFilter(filter).ToList();

        if (userSearch.Count == 0)
        {
            return new DataOutput<IList<UserDto>>([], ["No users found for the given filter"], true);
        }

        var users = userSearch.Select(user => user.ToDto()).ToList();

        return new DataOutput<IList<UserDto>>(users, ["Search completed with success"], true);
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
    
    public ProcessOutput ActivateManyUsers(int[] ids)
    {
        List<int> notFoundIds = [];
        List<(int, string[])> canNotActivateIds = [];
        
        foreach (var id in ids)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);
                
                continue;
            }

            var canActivate = user.CanActivate();

            if (!canActivate.Success)
            {
                canNotActivateIds.Add((id, canActivate.Errors.ToArray()));
                
                continue;
            }

            user.Active = true;
            userRepository.Update(user);
        }
        
        var notFoundMessage = notFoundIds.Count > 0 ? $"Users with IDs {string.Join(", ", notFoundIds)} not found" : string.Empty;
        var canNotActivateMessage = canNotActivateIds.Count > 0 ? $"Users with IDs {string.Join(", ", canNotActivateIds.Select(x => x.Item1))} cannot be activated" : string.Empty;

        List<string> errors = [];
        
        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }
        
        if (!string.IsNullOrEmpty(canNotActivateMessage))
        {
            errors.Add(canNotActivateMessage);
            errors.AddRange(canNotActivateIds.SelectMany(x => x.Item2));
        }

        return new ProcessOutput(errors);
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
    
    public ProcessOutput DeactivateManyUsers(int[] ids)
    {
        List<int> notFoundIds = [];
        List<(int, string[])> canNotDeactivateIds = [];
        
        foreach (var id in ids)
        {
            var user = userRepository.GetById(id);

            if (user is null)
            {
                notFoundIds.Add(id);
                
                continue;
            }

            var canDeactivate = user.CanDeactivate();

            if (!canDeactivate.Success)
            {
                canNotDeactivateIds.Add((id, canDeactivate.Errors.ToArray()));
                
                continue;
            }

            user.Active = false;
            userRepository.Update(user);
        }
        
        var notFoundMessage = notFoundIds.Count > 0 ? $"Users with IDs {string.Join(", ", notFoundIds)} not found" : string.Empty;
        var canNotDeactivateMessage = canNotDeactivateIds.Count > 0 ? $"Users with IDs {string.Join(", ", canNotDeactivateIds.Select(x => x.Item1))} cannot be deactivated" : string.Empty;

        List<string> errors = [];
        
        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }
        
        if (!string.IsNullOrEmpty(canNotDeactivateMessage))
        {
            errors.Add(canNotDeactivateMessage);
            errors.AddRange(canNotDeactivateIds.SelectMany(x => x.Item2));
        }

        return new ProcessOutput(errors);
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

    public ProcessOutput DeleteManyUsers(int[] ids)
    {
        List<int> idsToDelete = [];
        List<int> notFoundIds = [];
        List<(int, string[])> canNotDeleteIds = [];

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
                canNotDeleteIds.Add((id, canDelete.Errors.ToArray()));
                
                continue;
            }
            
            idsToDelete.Add(id);
        }
        
        userRepository.MultiDelete(idsToDelete);
        
        var notFoundMessage = notFoundIds.Count > 0 ? $"Users with IDs {string.Join(", ", notFoundIds)} not found" : string.Empty;
        var canNotDeleteMessage = canNotDeleteIds.Count > 0 ? $"Users with IDs {string.Join(", ", canNotDeleteIds.Select(x => x.Item1))} cannot be deleted" : string.Empty;

        List<string> errors = [];
        
        if (!string.IsNullOrEmpty(notFoundMessage))
        {
            errors.Add(notFoundMessage);
        }
        
        if (!string.IsNullOrEmpty(canNotDeleteMessage))
        {
            errors.Add(canNotDeleteMessage);
            errors.AddRange(canNotDeleteIds.SelectMany(x => x.Item2));
        }

        return new ProcessOutput(errors);
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
