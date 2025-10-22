using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Data;
using ArturRios.UserManagement.Data.Client.Configuration;
using ArturRios.UserManagement.Data.Client.Repositories;

namespace ArturRios.UserManagement.Data.Client;

public class DataClient : BaseDataClient
{
    public DataClient(EnvironmentType environment) : base(environment) =>
        UserRepository = new UserRepository(new ClientDbContextFactory());

    public DataClient(string connectionString) : base(connectionString) =>
        UserRepository = new UserRepository(new ClientDbContextFactory());

    public UserRepository UserRepository { get; }
}
