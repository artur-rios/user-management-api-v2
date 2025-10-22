using ArturRios.Common.Web.Api.Client;
using ArturRios.UserManagement.WebApi.Client.Routes;

namespace ArturRios.UserManagement.WebApi.Client;

public class WebApiClient : BaseWebApiClient
{
    public WebApiClient(HttpClient httpClient) : base(httpClient) { }

    public WebApiClient(string baseUrl) : base(baseUrl) { }
    public AuthenticationRoute Authentication { get; private set; } = null!;
    public HealthCheckRoute HealthCheck { get; private set; } = null!;
    public UserRoute User { get; private set; } = null!;

    protected override void SetRoutes()
    {
        Authentication = new AuthenticationRoute(Gateway);
        HealthCheck = new HealthCheckRoute(Gateway);
        User = new UserRoute(Gateway);
    }
}
