using System.Net;
using ArturRios.Common.Environment;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;

namespace ArturRios.UserManagement.WebApi.Tests;

public class HealthCheckTests(EnvironmentType environment = EnvironmentType.Local) : WebApiTest<Program>(environment)
{
    private const string HealthCheckRoute = "/HealthCheck";

    [FunctionalFact]
    public async Task Should_DoHealthCheck()
    {
        var output = await Get<string>(HealthCheckRoute, HttpStatusCode.OK);
        
        Assert.NotNull(output);
        Assert.Equal("Hello world!", output.Data);
        Assert.Equal("User management Web API is ON", output.Messages.First());
    }
}
