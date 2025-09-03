using ArturRios.Common.WebApi.Client;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class AuthenticationRoute(HttpClient httpClient) : BaseWebApiClientRoute(httpClient)
{
    public override string BaseUrl => "/Authentication";
}