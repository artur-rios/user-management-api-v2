using System.Net;
using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Output;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;

namespace ArturRios.UserManagement.WebApi.Tests;

public class HealthCheckTests(EnvironmentType environment = EnvironmentType.Local) : WebApiTest<Program>(environment)
{
    private const string HealthCheckRoute = "/HealthCheck";

    [FunctionalFact]
    public async Task Should_DoHealthCheck()
    {
        var output = await Gateway.GetAsync<DataOutput<string>>(HealthCheckRoute);

        Assert.Equal(HttpStatusCode.OK, output.StatusCode);
        Assert.NotNull(output.Body?.Data);
        Assert.Equal("Hello world!", output.Body.Data);
        Assert.Equal("User management Web API is ON", output.Body.Messages.First());
    }
}
