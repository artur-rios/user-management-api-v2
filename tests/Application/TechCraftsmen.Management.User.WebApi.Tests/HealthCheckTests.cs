using System.Net;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class HealthCheckTests(EnvironmentType environment = EnvironmentType.Local) : WebApiTest<Program>(environment)
{
    private const string HealthCheckRoute = "/HealthCheck";

    [Functional]
    public async Task Should_DoHealthCheck()
    {
        var output = await Get<string>(HealthCheckRoute, HttpStatusCode.OK);
        
        Assert.NotNull(output);
        Assert.Equal("Hello world!", output.Data);
        Assert.Equal("User management Web API is ON", output.Messages.First());
    }
}
