using ArturRios.Common.Web.Api.Client;
using ArturRios.Common.Web.Api.Output;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class HealthCheckRoute(HttpClient httpClient) : BaseWebApiClientRoute(httpClient)
{
    public override string BaseUrl => "/HealthCheck";

    public async Task<WebApiOutput<string>?> HelloWorld()
    {
        return await GetAsync<string>(BaseUrl);
    }
}
