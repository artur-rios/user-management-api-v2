using ArturRios.Common.Configuration;
using ArturRios.UserManagement.Data.Client.Configuration;
using ArturRios.UserManagement.Data.Client.Repositories;

namespace ArturRios.UserManagement.Data.Client;

public class DataClient
{
    public UserRepository UserRepository { get; }

    public DataClient(EnvironmentType environment)
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environment.ToString());
        
        UserRepository = new UserRepository(new ClientDbContextFactory());
    }
}
