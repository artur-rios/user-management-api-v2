using System.Net;
using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;

namespace ArturRios.UserManagement.WebApi.Client.Tests;

public class HealthCheckTests : WebApiTest<Program>
{
    private readonly WebApiClient _webApiClient;

    public HealthCheckTests(EnvironmentType environment = EnvironmentType.Local) : base(environment)
    {
        _webApiClient = new WebApiClient(Gateway.Client);
    }

    [FunctionalFact]
    public async Task Should_DoHealthCheck()
    {
        var output = await _webApiClient.HealthCheck.HelloWorld();

        Assert.Equal(HttpStatusCode.OK, output.StatusCode);
        Assert.NotNull(output.Body?.Data);
        Assert.Equal("Hello world!", output.Body.Data);
        Assert.Equal("User management Web API is ON", output.Body.Messages.First());
    }
}
