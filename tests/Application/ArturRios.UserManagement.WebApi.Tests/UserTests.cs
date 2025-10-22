using System.Net;
using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Extensions;
using ArturRios.Common.Output;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;
using ArturRios.Common.Util.Random;
using ArturRios.Common.Web.Security.Records;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Test.Fixture;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.WebApi.Tests;

public class UserTests(
    DatabaseFixture fixture,
    EnvironmentType environment = EnvironmentType.Local)
    : WebApiTest<Program>(environment), IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private const string AuthenticationRoute = "/Authentication";
    private const string UserRoute = "/User";

    private readonly List<int> _userIdsToDelete = [];

    private User _testUser = new();

    public async Task InitializeAsync()
    {
        _testUser = fixture.CreateUsers().First();

        Credentials credentials = new() { Email = _testUser.Email, Password = fixture.GetPassword(_testUser.Id) };

        await AuthenticateAndAuthorizeAsync(credentials, AuthenticationRoute);
    }

    public Task DisposeAsync()
    {
        fixture.DeleteUsers(_userIdsToDelete.ToArray());

        return Task.CompletedTask;
    }

    [FunctionalFact]
    public async Task Should_CreateUser()
    {
        var user = UserMock.New.WithNoId().WithRole(Roles.Regular).Generate();
        var password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var command = new CreateUserCommand
        {
            Name = user.Name, Email = user.Email, RoleId = user.RoleId, Password = password
        };

        var result =
            await Gateway.PostAsync<DataOutput<CreateUserCommandOutput>>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.True(result.Body.Data.CreatedUserId > 0);
        Assert.True(result.Body.Success);

        _userIdsToDelete.Add(result.Body.Data.CreatedUserId);

        Assert.Equal("User created successfully", result.Body.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotCreateUser_WhenInputIsInvalid()
    {
        var user = UserMock.New.WithNoId().WithEmail(string.Empty).WithRole(Roles.Regular).Generate();
        var password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var command = new CreateUserCommand
        {
            Name = user.Name, Email = user.Email, RoleId = user.RoleId, Password = password
        };

        var result =
            await Gateway.PostAsync<DataOutput<CreateUserCommandOutput>>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Email should be valid", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_NotCreateUser_WhenEmailAlreadyRegistered()
    {
        var user = fixture.CreateUsers().First();
        var password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var command = new CreateUserCommand
        {
            Name = user.Name, Email = user.Email, RoleId = user.RoleId, Password = password
        };

        var result =
            await Gateway.PostAsync<DataOutput<CreateUserCommandOutput>>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("E-mail already registered", result.Body?.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_GetUserById()
    {
        var result = await Gateway.GetAsync<DataOutput<UserQueryOutput>>($"{UserRoute}/{_testUser.Id}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.True(result.Body.Success);
        Assert.Equal($"User with Id '{_testUser.Id}' found", result.Body.Messages.First());

        var user = result.Body.Data;

        Assert.Equal(_testUser.Id, user?.Id);
        Assert.Equal(_testUser.Name, user?.Name);
        Assert.Equal(_testUser.Email, user?.Email);
        Assert.Equal(_testUser.RoleId, user?.RoleId);
        Assert.True(user?.Active);
        Assert.Equal(_testUser.CreatedAt.RemoveMilliseconds(), user?.CreatedAt.RemoveMilliseconds());
    }

    [FunctionalFact]
    public async Task ShouldNot_GetUserById()
    {
        var nonExistentUserId = fixture.GetUserNextId();

        var result = await Gateway.GetAsync<PaginatedOutput<UserQueryOutput>>($"{UserRoute}/{nonExistentUserId}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.True(result.Body.Success);
        Assert.Equal($"User with Id '{nonExistentUserId}' not found", result.Body?.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_GetUsersByFilter()
    {
        var query = $"?Name={_testUser.Name}";

        var result = await Gateway.GetAsync<PaginatedOutput<UserQueryOutput>>($"{UserRoute}/Filter{query}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.True(result.Body.Success);

        var userFound = result.Body.Data.FirstOrDefault();

        Assert.Equal(_testUser.Id, userFound?.Id);
        Assert.Equal(_testUser.Name, userFound?.Name);
        Assert.Equal(_testUser.Email, userFound?.Email);
        Assert.Equal(_testUser.RoleId, userFound?.RoleId);
        Assert.True(userFound?.Active);
        Assert.Equal(_testUser.CreatedAt.RemoveMilliseconds(), userFound?.CreatedAt.RemoveMilliseconds());
    }

    [FunctionalFact]
    public async Task ShouldNot_GetUsersByFilter()
    {
        const string query = "?Name=999999";

        var result = await Gateway.GetAsync<PaginatedOutput<UserQueryOutput>>($"{UserRoute}/Filter{query}");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.Empty(result.Body.Data);
        Assert.True(result.Body.Success);
    }

    [FunctionalFact]
    public async Task Should_UpdateUser()
    {
        var user = fixture.CreateUsers().First();
        const string updatedName = "Updated name";

        var command = new UpdateUserCommand { Id = user.Id, Name = updatedName };

        var updateResult = await Gateway.PutAsync<DataOutput<UpdateUserCommandOutput>>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.OK, updateResult.StatusCode);
        Assert.NotNull(updateResult.Body?.Data);
        Assert.Equal(user.Id, updateResult.Body.Data.Id);
        Assert.Equal(updatedName, updateResult.Body.Data.Name);
        Assert.Equal("User updated with success", updateResult.Body.Messages.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(updatedName, returnedUser.Name);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_InputIsInvalid()
    {
        var user = fixture.CreateUsers().First();
        user.Name = string.Empty;

        var command = new UpdateUserCommand { Id = user.Id, Name = user.Name };

        var result = await Gateway.PutAsync<DataOutput<UpdateUserCommandOutput>>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Name should be valid", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_UserIsInactive()
    {
        var user = fixture.CreateUsers(false).First();

        var command = new UpdateUserCommand { Id = user.Id, Name = "Updated name" };

        var result = await Gateway.PutAsync<DataOutput<UpdateUserCommandOutput>>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Can't update inactive user", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_UserDoesNotExist()
    {
        var command = new UpdateUserCommand { Id = fixture.GetUserNextId(), Name = "Updated name" };

        var result = await Gateway.PutAsync<DataOutput<UpdateUserCommandOutput>>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("User not found", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_UpdateUserRole()
    {
        var user = fixture.CreateUsers().First();

        var newRoleId = user.RoleId == (int)Roles.Test ? (int)Roles.Admin : (int)Roles.Test;

        var command = new UpdateUserRoleCommand { UserId = user.Id, NewRoleId = newRoleId };

        var result =
            await Gateway.PatchAsync<DataOutput<UpdateUserRoleCommandOutput>>($"{UserRoute}/Update/Role", command);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.Equal(newRoleId, result.Body.Data.UpdatedUserRoleId);
        Assert.Equal($"User role updated to {newRoleId}", result.Body.Messages.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(newRoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_RoleIsInvalid()
    {
        var user = fixture.CreateUsers().First();

        var command = new UpdateUserRoleCommand { UserId = user.Id, NewRoleId = 999 };

        var result =
            await Gateway.PatchAsync<DataOutput<UpdateUserRoleCommandOutput>>($"{UserRoute}/Update/Role", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Role should be valid", result.Body.Errors.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.RoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_UserIsInactive()
    {
        var user = fixture.CreateUsers(false).First();

        var newRoleId = user.RoleId == (int)Roles.Test ? (int)Roles.Admin : (int)Roles.Test;

        var command = new UpdateUserRoleCommand { UserId = user.Id, NewRoleId = newRoleId };

        var result =
            await Gateway.PatchAsync<DataOutput<UpdateUserRoleCommandOutput>>($"{UserRoute}/Update/Role", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Can't change role of inactive user", result.Body.Errors.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.RoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_UserDoesNotExist()
    {
        var nonExistentUserId = fixture.GetUserNextId();
        const int newRoleId = (int)Roles.Admin;

        var command = new UpdateUserRoleCommand { UserId = nonExistentUserId, NewRoleId = newRoleId };

        var result =
            await Gateway.PatchAsync<DataOutput<UpdateUserRoleCommandOutput>>($"{UserRoute}/Update/Role", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("User not found", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await Gateway.PatchAsync<DataOutput<ActivateUserCommandOutput>>($"{UserRoute}/{userId}/Activate");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.True(result.Body.Success);
        Assert.Equal("User activated successfully", result.Body.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotActivateUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await Gateway.PatchAsync<DataOutput<ActivateUserCommandOutput>>($"{UserRoute}/{userId}/Activate");

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("User already active", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateManyUsers()
    {
        const int quantityToActivate = 3;
        var ids = fixture.CreateUsers(false, quantityToActivate).Select(user => user.Id).ToArray();

        var result =
            await Gateway.PatchAsync<DataOutput<ActivateManyUsersCommandOutput>>($"{UserRoute}/ActivateMany", ids);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body?.Data);
        Assert.Equal(quantityToActivate, result.Body.Data.ActivatedIds.Count());
        Assert.Empty(result.Body.Data.FailedActivationIds);
        Assert.Empty(result.Body.Data.FailedActivationIds);
        Assert.True(result.Body.Success);
        Assert.Equal($"{quantityToActivate} user(s) activated successfully", result.Body.Messages.First());

        var users = fixture.GetAllUsers()
            .Where(user => ids.Contains(user.Id))
            .Where(user => user.Active)
            .ToList();

        Assert.Equal(quantityToActivate, users.Count);

        foreach (var user in users)
        {
            Assert.True(user.Active);
        }
    }

    [FunctionalFact]
    public async Task Should_DeactivateUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result =
            await Gateway.PatchAsync<DataOutput<DeactivateUserCommandOutput>>($"{UserRoute}/{userId}/Deactivate");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.True(result.Body.Success);
        Assert.Equal("User deactivated successfully", result.Body.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeactivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result =
            await Gateway.PatchAsync<DataOutput<DeactivateUserCommandOutput>>($"{UserRoute}/{userId}/Deactivate");

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("User already inactive", result.Body.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_DeleteUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await Gateway.DeleteAsync<DataOutput<DeleteUserCommandOutput>>($"{UserRoute}/{userId}/Delete");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.True(result.Body.Success);
        Assert.Equal("User deleted successfully", result.Body.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeleteUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await Gateway.DeleteAsync<DataOutput<DeleteUserCommandOutput>>($"{UserRoute}/{userId}/Delete");

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.NotNull(result.Body);
        Assert.Null(result.Body.Data);
        Assert.False(result.Body.Success);
        Assert.Equal("Can't delete active user", result.Body.Errors.First());
    }
}
