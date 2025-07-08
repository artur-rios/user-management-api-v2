using ArturRios.Common.Extensions;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Domain.Filters;
using ArturRios.UserManagement.Domain.Interfaces;
using ArturRios.UserManagement.Dto.Validation;
using ArturRios.UserManagement.Test.Mock;
using Bogus;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ArturRios.UserManagement.Services.Tests;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Faker _faker = new();

    private static List<Domain.Aggregates.User> ActiveUsers =>
    [
        UserMock.New.WithId(1).Active().Generate(),
        UserMock.New.WithId(2).Active().Generate(),
        UserMock.New.WithId(3).Active().Generate()
    ];

    private static List<Domain.Aggregates.User> InactiveUsers =>
    [
        UserMock.New.WithId(4).Inactive().Generate(),
        UserMock.New.WithId(5).Inactive().Generate(),
        UserMock.New.WithId(6).Inactive().Generate()
    ];

    private static int[] ActiveIds => ActiveUsers.Select(user => user.Id).ToArray();
    private static readonly int[] InactiveIds = InactiveUsers.Select(user => user.Id).ToArray();
    private static readonly int[] NonexistentIds = [7, 8, 9];

    private const string ActiveEmail = "active@mail.com";
    private const string InactiveEmail = "inactive@mail.com";

    public UserServiceTests()
    {
        var userRepository = new Mock<IUserRepository>();
        var httpContextAccessor = new Mock<HttpContextAccessor>();
        var userDtoValidator = new UserDtoValidator();

        userRepository.Setup(repo => repo.Create(It.IsAny<Domain.Aggregates.User>()))
            .Returns(() => _faker.Random.Int(1, 1000));

        userRepository.Setup(repo => repo.GetById(It.IsAny<int>(), false))
            .Returns((int id, bool _) =>
            {
                if (ActiveIds.Contains(id))
                {
                    return UserMock.New.WithId(id).Generate();
                }

                return InactiveUsers.Select(user => user.Id).Contains(id)
                    ? UserMock.New.WithId(id).WithEmail(InactiveEmail).Inactive().Generate()
                    : null;
            });

        userRepository.Setup(repo => repo.GetByFilter(It.IsAny<UserFilter>(), false))
            .Returns((UserFilter userFilter, bool _) =>
            {
                if (userFilter.Active.HasValue && userFilter.Active.Value)
                {
                    return ActiveUsers.AsQueryable();
                }
                
                if (userFilter.Email == ActiveEmail)
                {
                    return new List<Domain.Aggregates.User> { UserMock.New.WithEmail(ActiveEmail).Generate() }
                        .AsQueryable();
                }
                
                return new List<Domain.Aggregates.User>().AsQueryable();
            });

        userRepository.Setup(r => r.GetByMultiFilter(It.IsAny<UserMultiFilter>(), false))
            .Returns((UserMultiFilter filter, bool _) => filter.Ids!.SequenceEqual(ActiveIds)
                ? new List<Domain.Aggregates.User>
                    { UserMock.New.Generate(), UserMock.New.Generate(), UserMock.New.Generate() }.AsQueryable()
                : new List<Domain.Aggregates.User>().AsQueryable()
            );

        userRepository.Setup(repo => repo.Update(It.IsAny<Domain.Aggregates.User>()));

        userRepository.Setup(repo => repo.Delete(It.IsAny<int>()));

        httpContextAccessor.Object.HttpContext = new DefaultHttpContext();
        httpContextAccessor.Object.HttpContext!.Items = new Dictionary<object, object?>
            { { "User", new AuthenticatedUserMock(Roles.Regular).Generate() } };

        _userService = new UserService(userRepository.Object, httpContextAccessor.Object, userDtoValidator);
    }

    [UnitFact]
    public void Should_CreateUser()
    {
        var dto = UserMock.New.WithRole(Roles.Regular).Generate().ToDtoWithPassword();

        var result = _userService.CreateUser(dto);

        Assert.True(result.Data > 0);
        Assert.Equal("User created with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_CreateUser_When_UserDtoIsInvalid()
    {
        var dto = UserMock.New.WithEmail("").Generate().ToDtoWithPassword();

        var result = _userService.CreateUser(dto);

        Assert.Equal(0, result.Data);
        Assert.Equal("Email should be valid", result.Messages.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_CreateUser_When_EmailIsAlreadyRegistered()
    {
        var dto = UserMock.New.WithEmail(ActiveEmail).Generate().ToDtoWithPassword();

        var result = _userService.CreateUser(dto);

        Assert.Equal(0, result.Data);
        Assert.Equal("E-mail already registered", result.Messages.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_CreateAdminUser_When_AuthenticatedUserIsNotAdmin()
    {
        var dto = UserMock.New.WithRole(Roles.Admin).Generate().ToDtoWithPassword();

        var result = _userService.CreateUser(dto);

        Assert.Equal(0, result.Data);
        Assert.Equal($"Only admins can register a user with {nameof(Roles.Admin)} role", result.Messages.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_GetUserById()
    {
        var result = _userService.GetUserById(ActiveIds.First());

        Assert.NotNull(result.Data);
        Assert.Equal("User found", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_GetUserById()
    {
        var result = _userService.GetUserById(NonexistentIds.First());

        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void Should_GetUserByFilter()
    {
        var filter = new UserFilter { Email = ActiveEmail };
        var result = _userService.GetUsersByFilter(filter);

        Assert.NotEmpty(result.Data!);
        Assert.Single(result.Data!);
        Assert.Equal("Search completed with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void Should_GetUsersByFilter()
    {
        var filter = new UserFilter { Active = true};
        var result = _userService.GetUsersByFilter(filter);

        Assert.NotEmpty(result.Data!);
        Assert.Equal(ActiveUsers.Count, result.Data!.Count);
        Assert.Equal("Search completed with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_GetUsers_When_FilterDoesNotMatchAnyUser()
    {
        var filter = new UserFilter { Email = InactiveEmail };
        var result = _userService.GetUsersByFilter(filter);

        Assert.Empty(result.Data!);
        Assert.Equal("No users found for the given filter", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void Should_GetUsersByMultiFilter()
    {
        var filter = new UserMultiFilter
        {
            Ids = ActiveIds
        };

        var result = _userService.GetUsersByMultiFilter(filter);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
        Assert.Contains("Search completed with success", result.Messages);
    }

    [UnitFact]
    public void ShouldNot_GetUsersByMultiFilter()
    {
        var filter = new UserMultiFilter
        {
            Ids = NonexistentIds
        };

        var result = _userService.GetUsersByMultiFilter(filter);

        Assert.True(result.Success);
        CustomAssert.NullOrEmpty(result.Data);
        Assert.Contains("No users found for the given filter", result.Messages);
    }

    [UnitFact]
    public void Should_UpdateUser()
    {
        var dto = UserMock.New.WithId(ActiveIds.First()).Generate().ToDtoWithPassword();

        var result = _userService.UpdateUser(dto);

        Assert.NotNull(result.Data);
        Assert.Equal("User updated with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void Should_NotUpdateUser_When_UserIsInactive()
    {
        var dto = UserMock.New.WithId(InactiveIds.First()).Inactive().Generate().ToDtoWithPassword();

        var result = _userService.UpdateUser(dto);

        Assert.Null(result.Data);
        Assert.Equal("Can't update inactive user", result.Messages.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_NotUpdateUser_When_IdIsNotOnDatabase()
    {
        var dto = UserMock.New.WithId(NonexistentIds.First()).Generate().ToDtoWithPassword();

        var result = _userService.UpdateUser(dto);

        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Messages.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_ActivateUser()
    {
        var result = _userService.ActivateUser(InactiveIds.First());

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_ActivateUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.ActivateUser(NonexistentIds.First());

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_ActivateUser_When_UserIsAlreadyActive()
    {
        var result = _userService.ActivateUser(ActiveIds.First());

        Assert.Equal("User already active", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_ActivateUsers()
    {
        var result = _userService.ActivateManyUsers(InactiveIds);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
    }

    [UnitFact]
    public void Should_Not_ActivateUsers_When_UsersAreAlreadyActive()
    {
        var result = _userService.ActivateManyUsers(ActiveIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        for (var i = 0; i < ActiveIds.Length; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", ActiveIds)} cannot be activated", result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {ActiveIds[i]}: User already active",
                result.Errors.Skip(1).FirstOrDefault(e => ExtractIdFromDomainError(e) == ActiveIds[i]));
        }
    }

    [UnitFact]
    public void ShouldNot_ActivateUsers_When_IdsAreNotOnDatabase()
    {
        var result = _userService.ActivateManyUsers(NonexistentIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", NonexistentIds)} not found", result.Errors.First());
    }

    [UnitFact]
    public void Should_DeactivateUser()
    {
        var result = _userService.DeactivateUser(ActiveIds.First());

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeactivateUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.DeactivateUser(NonexistentIds.First());

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeactivateUser_When_UserIsAlreadyInactive()
    {
        var result = _userService.DeactivateUser(InactiveIds.First());

        Assert.Equal("User already inactive", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_DeactivateUsers()
    {
        var result = _userService.DeactivateManyUsers(ActiveIds);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
    }

    [UnitFact]
    public void Should_NotDeactivateUsers_When_UsersAreAlreadyInactive()
    {
        var result = _userService.DeactivateManyUsers(InactiveIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        for (var i = 0; i < InactiveIds.Length; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", InactiveIds)} cannot be deactivated",
                    result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {InactiveIds[i]}: User already inactive",
                result.Errors.Skip(1).FirstOrDefault(e => ExtractIdFromDomainError(e) == InactiveIds[i]));
        }
    }

    [UnitFact]
    public void Should_Not_DeactivateUsers_When_IdsAreNotOnDatabase()
    {
        var result = _userService.DeactivateManyUsers(NonexistentIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", NonexistentIds)} not found", result.Errors.First());
    }

    [UnitFact]
    public void Should_DeleteUser()
    {
        var result = _userService.DeleteUser(InactiveIds.First());

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUser_When_UserIsActive()
    {
        var result = _userService.DeleteUser(ActiveIds.First());

        Assert.Equal("Can't delete active user", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.DeleteUser(NonexistentIds.First());

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_DeleteUsers()
    {
        var result = _userService.DeleteManyUsers(InactiveIds);

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUsers_When_UsersAreActive()
    {
        var result = _userService.DeleteManyUsers(ActiveIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", ActiveIds)} cannot be deleted", result.Errors.First());

        for (var i = 0; i < ActiveIds.Length; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", ActiveIds)} cannot be deleted", result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {ActiveIds[i]}: Can't delete active user",
                result.Errors.Skip(1).FirstOrDefault(e => ExtractIdFromDomainError(e) == ActiveIds[i]));
        }
    }

    [UnitFact]
    public void ShouldNot_DeleteUsers_When_IdsAreNotOnDatabase()
    {
        var result = _userService.DeleteManyUsers(NonexistentIds);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", NonexistentIds)} not found", result.Errors.First());
    }

    private static int ExtractIdFromDomainError(string error)
    {
        return int.Parse(error.Split(" ")[3].TrimChar(':'));
    }
}