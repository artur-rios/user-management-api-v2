using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Configuration.Loaders;
using ArturRios.UserManagement.Data.Client.Configuration;
using ArturRios.UserManagement.Data.Client.Repositories;

namespace ArturRios.UserManagement.Data.Client;

public class DataClient
{
    public UserRepository UserRepository { get; }

    public DataClient(EnvironmentType environment)
    {
        var configLoader = new ConfigurationLoader(environment.ToString());
        configLoader.LoadEnvironment();
        
        UserRepository = new UserRepository(new ClientDbContextFactory());
    }

    public DataClient(string connectionString)
    {
        Environment.SetEnvironmentVariable("DATABASE_CONNECTION_STRING", connectionString);
        
        UserRepository = new UserRepository(new ClientDbContextFactory());
    }
}
