using ArturRios.Common.Web.Api.Client;
using ArturRios.Common.Web.Http;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class UserRoute(HttpGateway gateway) : BaseWebApiClientRoute(gateway)
{
    public override string BaseUrl => "User";
}
