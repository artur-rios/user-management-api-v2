using System.Net;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class AuthenticationTests(EnvironmentType environment = EnvironmentType.Local) : WebApiTest<Program>(environment)
{
    private const string AuthenticationRoute = "/Authentication";
    
    [Functional]
    public async Task Should_AuthenticateUser()
    {
        var credentials = CredentialsMock.New.Generate();
        
        var output = await Post<Authentication>(AuthenticationRoute, credentials, HttpStatusCode.OK);
        
        Assert.NotNull(output);
        CustomAssert.NotNullOrWhiteSpace(output.Data?.Token);
        Assert.True(output.Data?.Valid);
    }
}