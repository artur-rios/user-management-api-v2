using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Client;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class HealthCheckRoute(HttpClient httpClient) : BaseWebApiClientRoute(httpClient)
{
    public override string BaseUrl => "/HealthCheck";

    public async Task<WebApiOutput<string>?> HelloWorld()
    {
        return await GetAsync<string>(BaseUrl);
    }
}