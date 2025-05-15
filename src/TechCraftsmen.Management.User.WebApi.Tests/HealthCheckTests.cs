using System.Net;
using Newtonsoft.Json;
using TechCraftsmen.Core.Test;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Core.WebApi;

namespace TechCraftsmen.Management.User.WebApi.Tests;

public class HealthCheckTests(string environment = "local") : WebApiTest<Program>(environment)
{
    private const string HealthCheckRoute = "/HealthCheck";

    [Functional]
    public void Should_DoHealthCheck_And_ReturnSuccess()
    {
        var response = Client.GetAsync(HealthCheckRoute).Result;

        var body = response.Content.ReadAsStringAsync().Result;

        var result = JsonConvert.DeserializeObject<WebApiOutput<string>>(body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Hello world!", result.Data);
        Assert.Equal("User Api ON", result.Messages.First());
    }
}