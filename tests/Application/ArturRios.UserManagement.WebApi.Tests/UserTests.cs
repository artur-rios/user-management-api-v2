using System.Net;
using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Extensions;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;
using ArturRios.Common.Util.Random;
using ArturRios.Common.WebApi.Security.Records;
using ArturRios.UserManagement.Domain;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Domain.Filters;
using ArturRios.UserManagement.Dto;
using ArturRios.UserManagement.Dto.Mapping;
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
    
    private UserDto _testUser = new();

    private readonly List<int> _userIdsToDelete = [];

    public async Task InitializeAsync()
    {
        _testUser = fixture.CreateUsers().First();
        
        Credentials credentials = new()
        {
            Email = _testUser.Email,
            Password = _testUser.Password
        };

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
        var user = UserMock.New.WithNoId().WithRole(Roles.Regular).Generate().ToDto();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var result = await PostAsync<int>($"{UserRoute}/Create/Regular", user);

        Assert.NotNull(result);
        Assert.True(result.Data > 0);
        Assert.True(result.Success);
        
        _userIdsToDelete.Add(result.Data);
        
        Assert.Equal("User created with success", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotCreateUser_WhenInputIsInvalid()
    {
        var user = UserMock.New.WithNoId().WithEmail(string.Empty).WithRole(Roles.Regular).Generate().ToDto();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });
        
        var result = await PostAsync<int>($"{UserRoute}/Create/Regular", user);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("Email should be valid", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotCreateUser_WhenEmailAlreadyRegistered()
    {
        var user = fixture.CreateUsers().First();
        user.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });
        
        var result = await PostAsync<int>($"{UserRoute}/Create/Regular", user);
        
        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("E-mail already registered", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_GetUserById()
    {
        var result = await GetAsync<UserDto>($"{UserRoute}/{_testUser.Id}", HttpStatusCode.OK);
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("User found", result.Messages.First());
        Assert.NotNull(result.Data);
        
        Assert.Equal(_testUser.Name, result.Data?.Name);
        Assert.Equal(_testUser.Email, result.Data?.Email);
        Assert.Equal(_testUser.RoleId, result.Data?.RoleId);
        Assert.True(result.Data?.Active);
    }
    
    [FunctionalFact]
    public async Task ShouldNot_GetUserById()
    {
        var nonExistentUserId = fixture.GetUserNextId();
        
        var result = await GetAsync<UserDto>($"{UserRoute}/{nonExistentUserId}", HttpStatusCode.OK);
        
        Assert.NotNull(result);
        Assert.Null(result.Data);
        Assert.True(result.Success);
        Assert.Equal("User not found", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_GetUsersByFilter()
    {
        var query = $"?Email={_testUser.Email}";

        var result = await GetAsync<IList<UserDto>>($"{UserRoute}/Filter{query}", HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Search completed with success", result.Messages.First());
        Assert.NotNull(result.Data);

        var userFound = result.Data.FirstOrDefault();

        Assert.Equal(_testUser.Name, userFound?.Name);
        Assert.Equal(_testUser.Email, userFound?.Email);
        Assert.Equal(_testUser.RoleId, userFound?.RoleId);
    }

    [FunctionalFact]
    public async Task ShouldNot_GetUsersByFilter()
    {
        const string query = $"?Email={NonexistentEmail}";

        var result = await GetAsync<IList<UserDto>>($"{UserRoute}/Filter{query}", HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data!);
        Assert.Equal("No users found for the given filter", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_DeactivateUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await PatchAsync<string>($"{UserRoute}/{userId}/Deactivate", null, HttpStatusCode.OK);

        Assert.NotNull(result);
        Assert.Equal($"User with id {userId} deactivated successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotDeactivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await PatchAsync<string>($"{UserRoute}/{userId}/Deactivate");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("User already inactive", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await PatchAsync<string>($"{UserRoute}/{userId}/Activate");

        Assert.NotNull(result);
        Assert.Equal($"User with id {userId} activated successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_NotActivateUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await PatchAsync<string>($"{UserRoute}/{userId}/Activate");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("User already active", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ActivateMultipleUsers()
    {
        var ids = fixture.CreateUsers(false, 3).Select(user => user.Id).ToArray();
        
        var result = await PatchAsync<string>($"{UserRoute}/ActivateMany", ids, HttpStatusCode.OK);
        
        Assert.NotNull(result);
        Assert.Equal("All users activated successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());
        
        var users = fixture.GetUsersByMultiFilter(new UserMultiFilter { Ids = ids.ToList() }).ToList();
        
        Assert.Equal(3, users.Count);

        foreach (var user in users)
        {
            Assert.True(user.Active);
        }
    }

    [FunctionalFact]
    public async Task Should_NotDeleteUser()
    {
        var userId = fixture.CreateUsers().First().Id;

        var result = await DeleteAsync<string>($"{UserRoute}/{userId}/Delete");

        Assert.NotNull(result);
        Assert.Equal("Process executed with 1 error", result.Data);
        Assert.Equal("Can't delete active user", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_DeleteUser()
    {
        var userId = fixture.CreateUsers(false).First().Id;

        var result = await DeleteAsync<string>($"{UserRoute}/{userId}/Delete");

        Assert.NotNull(result);
        Assert.Equal($"User with id {userId} deleted successfully", result.Data);
        Assert.Equal("Process executed with no errors", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ReturnValidationError_And_Not_CreateUser()
    {
        var user = UserMock.New.WithNoId().WithEmail(string.Empty).WithRole(Roles.Regular).Generate().ToDto();

        var result = await PostAsync<int>($"{UserRoute}/Create/Regular", user);

        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("Email should be valid", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_ReturnEmailAlreadyRegisteredError_And_NotCreateUser()
    {
        var user = _testUser.Clone();
        user!.Password = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var result = await PostAsync<int>($"{UserRoute}/Create/Regular", user);

        Assert.NotNull(result);
        Assert.Equal(0, result.Data);
        Assert.False(result.Success);
        Assert.Equal("E-mail already registered", result.Messages.First());
    }

    [FunctionalFact]
    public async Task Should_UpdateUser()
    {
        var user = fixture.CreateUsers().First();
        
        user.Name = "Updated name";

        var updateResult = await PutAsync<UserDto>($"{UserRoute}/Update", user);

        Assert.NotNull(updateResult);
        Assert.Equal(user.Id, updateResult.Data?.Id);
        Assert.Equal(user.Name, updateResult.Data?.Name);
        Assert.Equal("User updated with success", updateResult.Messages.First());

        var returnedUser = fixture.GetUserById(user.Id);

        Assert.NotNull(returnedUser);
        Assert.Equal(user.Id, returnedUser.Id);
        Assert.Equal(user.Name, returnedUser.Name);
    }
}
