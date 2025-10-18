using System.Net;
using ArturRios.Common.Configuration.Enums;
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

    private const string NonexistentEmail = "inexists@mail.com";

    private User _testUser = new();

    private readonly List<int> _userIdsToDelete = [];

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

        var result = await Gateway.PostAsync<CreateUserCommandOutput>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.True(result.Data.CreatedUserId > 0);
        Assert.True(result.Success);

        _userIdsToDelete.Add(result.Data.CreatedUserId);

        Assert.Equal("User created with success", result.Messages.First());
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

        var result = await Gateway.PostAsync<CreateUserCommandOutput>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.False(result.Success);
        Assert.Equal("Email should be valid", result.Errors.First());
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

        var result = await Gateway.PostAsync<CreateUserCommandOutput>($"{UserRoute}/Create/Regular", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.False(result.Success);
        Assert.Equal("E-mail already registered", result.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_GetUserById()
    {
        var result = await Gateway.GetAsync<UserQueryOutput>($"{UserRoute}/{_testUser.Id}");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Equal($"User with Id '{_testUser.Id}' found", result.Messages.First());

        Assert.Equal(_testUser.Id, result.Data?.Id);
        Assert.Equal(_testUser.Name, result.Data?.Name);
        Assert.Equal(_testUser.Email, result.Data?.Email);
        Assert.Equal(_testUser.RoleId, result.Data?.RoleId);
        Assert.True(result.Data?.Active);
        Assert.Equal(_testUser.CreatedAt, result.Data?.CreatedAt);
    }

    [FunctionalFact]
    public async Task ShouldNot_GetUserById()
    {
        var nonExistentUserId = fixture.GetUserNextId();

        var result = await Gateway.GetAsync<UserQueryOutput>($"{UserRoute}/{nonExistentUserId}");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.True(result.Success);
        Assert.Equal($"User with Id '{nonExistentUserId}' not found", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_GetUsersByFilter()
    {
        var query = $"?Email={_testUser.Email}";

        var result = await Gateway.GetAsync<IList<UserQueryOutput>>($"{UserRoute}/Filter{query}");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.True(result.Success);

        var userFound = result.Data.FirstOrDefault();

        Assert.Equal(_testUser.Id, userFound?.Id);
        Assert.Equal(_testUser.Name, userFound?.Name);
        Assert.Equal(_testUser.Email, userFound?.Email);
        Assert.Equal(_testUser.RoleId, userFound?.RoleId);
        Assert.True(userFound?.Active);
        Assert.Equal(_testUser.CreatedAt, userFound?.CreatedAt);
    }

    [FunctionalFact]
    public async Task ShouldNot_GetUsersByFilter()
    {
        const string query = $"?Email={NonexistentEmail}";

        var result = await Gateway.GetAsync<IList<UserQueryOutput>>($"{UserRoute}/Filter{query}");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.True(result.Success);
        Assert.Empty(result.Data);
    }

    [FunctionalFact]
    public async Task Should_UpdateUser()
    {
        var user = fixture.CreateUsers().First();

        var command = new UpdateUserCommand { Id = user.Id, Name = "Updated name" };

        var updateResult = await Gateway.PutAsync<UpdateUserCommandOutput>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.OK, updateResult.GetStatusCode());
        Assert.NotNull(updateResult);
        Assert.Equal(user.Id, updateResult.Data?.Id);
        Assert.Equal(user.Name, updateResult.Data?.Name);
        Assert.Equal("User updated with success", updateResult.Messages.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(user.Name, returnedUser.Name);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_InputIsInvalid()
    {
        var user = fixture.CreateUsers().First();
        user.Name = string.Empty;

        var command = new UpdateUserCommand { Id = user.Id, Name = user.Name };

        var result = await Gateway.PutAsync<UpdateUserCommandOutput>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("Email should be valid", result.Errors.First());
        Assert.False(result.Success);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_UserIsInactive()
    {
        var user = fixture.CreateUsers(false).First();

        var command = new UpdateUserCommand { Id = user.Id, Name = "Updated name" };

        var result = await Gateway.PutAsync<UpdateUserCommandOutput>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.Equal("Can't update inactive user", result.Errors.First());
        Assert.False(result.Success);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUser_When_UserDoesNotExist()
    {
        var command = new UpdateUserCommand { Id = fixture.GetUserNextId(), Name = "Updated name" };

        var result = await Gateway.PutAsync<UpdateUserCommandOutput>($"{UserRoute}/Update", command);

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [FunctionalFact]
    public async Task Should_UpdateUserRole()
    {
        var user = fixture.CreateUsers().First();

        var newRoleId = user.RoleId == (int)Roles.Test ? (int)Roles.Admin : (int)Roles.Test;

        var result =
            await Gateway.PatchAsync<UpdateUserRoleCommandOutput>($"{UserRoute}/Update/{user.Id}/Role/{newRoleId}");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Equal(newRoleId, result.Data.UpdatedUserRoleId);
        Assert.Equal($"User role updated to {newRoleId}", result.Messages.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(newRoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_RoleIsInvalid()
    {
        var user = fixture.CreateUsers().First();

        var result = await Gateway.PatchAsync<UpdateUserRoleCommandOutput>($"{UserRoute}/Update/{user.Id}/Role/999");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("Role should be valid", result.Errors.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.RoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_UserIsInactive()
    {
        var user = fixture.CreateUsers(false).First();

        var newRoleId = user.RoleId == (int)Roles.Test ? (int)Roles.Admin : (int)Roles.Test;

        var result =
            await Gateway.PatchAsync<UpdateUserRoleCommandOutput>($"{UserRoute}/Update/{user.Id}/Role/{newRoleId}");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("Can't change role of inactive user", result.Errors.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.RoleId, returnedUser.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_UpdateUserRole_When_UserDoesNotExist()
    {
        var nonExistentUserId = fixture.GetUserNextId();
        const int newRoleId = (int)Roles.Admin;

        var result =
            await Gateway.PatchAsync<UpdateUserRoleCommandOutput>(
                $"{UserRoute}/Update/{nonExistentUserId}/Role/{newRoleId}");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.NotNull(result);
        Assert.Equal("User not found", result.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await Gateway.PatchAsync<ActivateUserCommandOutput>($"{UserRoute}/{userId}/Activate");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("User activated successfully", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotActivateUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await Gateway.PatchAsync<ActivateUserCommandOutput>($"{UserRoute}/{userId}/Activate");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("User already active", result.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateManyUsers()
    {
        const int quantityToActivate = 3;
        var ids = fixture.CreateUsers(false, quantityToActivate).Select(user => user.Id).ToArray();

        var result = await Gateway.PatchAsync<ActivateManyUsersCommandOutput>($"{UserRoute}/ActivateMany", ids);

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.NotNull(result.Data);
        Assert.Equal(quantityToActivate, result.Data.ActivatedIds.Count());
        Assert.Empty(result.Data.FailedActivationIds);
        Assert.Empty(result.Data.FailedActivationIds);
        Assert.Equal($"{quantityToActivate} user(s) activated successfully", result.Messages.First());

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

        var result = await Gateway.PatchAsync<DeactivateUserCommandOutput>($"{UserRoute}/{userId}/Deactivate");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("User deactivated successfully", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeactivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await Gateway.PatchAsync<DeactivateUserCommandOutput>($"{UserRoute}/{userId}/Deactivate");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("User already inactive", result.Errors.First());
    }

    [FunctionalFact]
    public async Task Should_DeleteUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await Gateway.DeleteAsync<DeleteUserCommandOutput>($"{UserRoute}/{userId}/Delete");

        Assert.Equal(HttpStatusCode.OK, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("User deleted successfully", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeleteUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await Gateway.DeleteAsync<DeleteUserCommandOutput>($"{UserRoute}/{userId}/Delete");

        Assert.Equal(HttpStatusCode.BadRequest, result.GetStatusCode());
        Assert.Null(result.Data);
        Assert.Equal("Can't delete active user", result.Errors.First());
    }
}
