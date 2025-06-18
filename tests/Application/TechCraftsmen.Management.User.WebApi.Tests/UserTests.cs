using System.Net;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Core.Util.Random;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Domain;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Dto.Mapping;
using TechCraftsmen.Management.User.Test.Fixture;
using TechCraftsmen.Management.User.Test.Mock;
using Xunit.Abstractions;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class UserTests(
    DatabaseFixture fixture,
    ITestOutputHelper testOutputHelper,
    EnvironmentType environment = EnvironmentType.Local)
    : WebApiTest<Program>(environment), IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private const string AuthenticationRoute = "/Authentication";
    private const string UserRoute = "/User";

    private const string NonexistentEmail = "inexists@mail.com";

    private static bool _testUserActive;
    private static int _testUserId;
    private static bool _testUserCreated;

    public async Task InitializeAsync()
    {
        Credentials credentials = new()
        {
            Email = fixture.TestUser.Email,
            Password = fixture.TestPassword
        };

        await AuthenticateAndAuthorize(credentials, AuthenticationRoute);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [FunctionalFact]
    public async Task Should_GetUsersByFilter()
    {
        var query = $"?Email={fixture.TestUser.Email}";

        var result = await Get<IList<UserDto>>($"{UserRoute}/Filter{query}", HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Search completed with success", result.Messages.First());
        Assert.NotNull(result.Data);

        var userFound = result.Data.FirstOrDefault();

        Assert.Equal(fixture.TestUser.Name, userFound?.Name);
        Assert.Equal(fixture.TestUser.Email, userFound?.Email);
        Assert.Equal(fixture.TestUser.RoleId, userFound?.RoleId);
    }

    [FunctionalFact]
    public async Task Should_ReturnEmptyList()
    {
        const string query = $"?Email={NonexistentEmail}";

        var result = await Get<IList<UserDto>>($"{UserRoute}/Filter{query}", HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data!);
        Assert.Equal("No users found for the given filter", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_CreateUser()
    {
        var user = UserMock.New.WithNoId().WithRole(Roles.Regular).Generate().ToDto();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var result = await Post<int>($"{UserRoute}/Create", user);
        _testUserId = result.Data;

        Assert.NotNull(result);
        Assert.True(_testUserId > 0);
        Assert.Equal("User created with success", result.Messages.First());

        _testUserActive = true;
        _testUserCreated = false;
    }

    [FunctionalFact]
    public async Task Should_DeactivateUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (!_testUserActive)
        {
            testOutputHelper.WriteLine("Test user not active, running Should_ActivateUser test to activate it");

            await Should_ActivateUser();
        }

        var result = await Patch<string>($"{UserRoute}/{_testUserId}/Deactivate");

        Assert.NotNull(result);
        Assert.Equal($"User with id {_testUserId} deactivated successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());

        _testUserActive = false;
    }

    [FunctionalFact]
    public async Task Should_NotDeactivateUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (_testUserActive)
        {
            testOutputHelper.WriteLine("Test user active, running Should_DeactivateUser test to activate it");

            await Should_DeactivateUser();
        }

        var result = await Patch<string>($"{UserRoute}/{_testUserId}/Deactivate");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("User already inactive", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (_testUserActive)
        {
            testOutputHelper.WriteLine("Test user active, running Should_DeactivateUser test to activate it");

            await Should_DeactivateUser();
        }

        var result = await Patch<string>($"{UserRoute}/{_testUserId}/Activate");

        Assert.NotNull(result);
        Assert.Equal($"User with id {_testUserId} activated successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());

        _testUserActive = true;
    }

    [FunctionalFact]
    public async Task Should_NotActivateUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (!_testUserActive)
        {
            testOutputHelper.WriteLine("Test user not active, running Should_ActivateUser test to activate it");

            await Should_ActivateUser();
        }

        var result = await Patch<string>($"{UserRoute}/{_testUserId}/Activate");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("User already active", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeleteUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (!_testUserActive)
        {
            testOutputHelper.WriteLine("Test user not active, running Should_ActivateUser test to activate it");

            await Should_ActivateUser();
        }

        var result = await Delete<string>($"{UserRoute}/{_testUserId}/Delete");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("Can't delete active user", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_DeleteUser()
    {
        if (!_testUserCreated)
        {
            testOutputHelper.WriteLine("Test user not created, running Should_CreateUser test to create it");

            await Should_CreateUser();
        }

        if (_testUserActive)
        {
            await Should_DeactivateUser();
        }

        var result = await Delete<string>($"{UserRoute}/{_testUserId}/Delete");

        Assert.NotNull(result);
        Assert.Equal($"User with id {_testUserId} deleted successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ReturnValidationError_And_Not_CreateUser()
    {
        var user = UserMock.New.WithNoId().WithEmail(string.Empty).WithRole(Roles.Regular).Generate().ToDto();

        var result = await Post<int>($"{UserRoute}/Create", user);

        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("Email should be valid", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ReturnEmailAlreadyRegisteredError_And_NotCreateUser()
    {
        var user = fixture.TestUser.ToDto();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var result = await Post<int>($"{UserRoute}/Create", user);

        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("E-mail already registered", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_UpdateUser()
    {
        var user = UserMock.New.WithNoId().WithRole(Roles.Regular).Generate().ToDto();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var creationResult = await Post<int>($"{UserRoute}/Create", user);
        var userId = creationResult.Data;

        Assert.NotNull(creationResult);
        Assert.True(userId > 0);
        Assert.Equal("User created with success", creationResult.Messages.First());

        user.Id = userId;
        user.Name = "Updated name";

        var updateResult = await Put<UserDto>($"{UserRoute}/Update", user);

        Assert.NotNull(updateResult);
        Assert.Equal(user.Id, updateResult.Data?.Id);
        Assert.Equal(user.Name, updateResult.Data?.Name);
        Assert.Equal("User updated with success", updateResult.Messages.First());

        var getUserResult = await Get<UserDto>($"{UserRoute}/{user.Id}");

        Assert.NotNull(getUserResult);
        Assert.NotNull(getUserResult.Data);
        Assert.Equal(user.Id, getUserResult.Data.Id);
        Assert.Equal(user.Name, getUserResult.Data.Name);
    }
}
