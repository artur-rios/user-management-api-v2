using ArturRios.Common.Output;
using ArturRios.Common.Web.Api.Client;
using ArturRios.Common.Web.Http;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class HealthCheckRoute(HttpGateway gateway) : BaseWebApiClientRoute(gateway)
{
    public override string BaseUrl => "/HealthCheck";

    public async Task<HttpOutput<DataOutput<string?>?>> HelloWorld() =>
        await Gateway.GetAsync<DataOutput<string?>>(BaseUrl);
}
