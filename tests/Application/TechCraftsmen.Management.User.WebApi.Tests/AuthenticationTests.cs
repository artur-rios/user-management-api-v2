using System.Net;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Test.Fixture;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class AuthenticationTests(DatabaseFixture fixture, EnvironmentType environment = EnvironmentType.Local)
    : WebApiTest<Program>(environment), IClassFixture<DatabaseFixture>
{
    private const string AuthenticationRoute = "/Authentication";

    [Functional]
    public async Task Should_AuthenticateUser()
    {
        var credentials = CredentialsMock.New
            .WithEmail(fixture.TestUser.Email)
            .WithPassword(fixture.TestPassword)
            .Generate();

        var output = await Post<Authentication>(AuthenticationRoute, credentials, HttpStatusCode.OK);

        Assert.NotNull(output);
        CustomAssert.NotNullOrWhiteSpace(output.Data?.Token);
        Assert.True(output.Data?.Valid);
    }
}
