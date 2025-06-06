using System.Net;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Test.Fixture;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class UserTests(DatabaseFixture fixture, EnvironmentType environment = EnvironmentType.Local)
    : WebApiTest<Program>(environment), IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private const string AuthenticationRoute = "/Authentication";
    private const string UserRoute = "/User";
    
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
}