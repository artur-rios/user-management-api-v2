using ArturRios.Common.WebApi.Client;

namespace ArturRios.UserManagement.WebApi.Client.Routes;

public class UserRoute(HttpClient httpClient) : BaseWebApiClientRoute(httpClient)
{
    public override string BaseUrl => "User";
}