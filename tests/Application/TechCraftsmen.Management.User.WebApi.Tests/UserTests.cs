using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.WebApi.Security.Records;
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
}