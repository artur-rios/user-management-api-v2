using ArturRios.Common.Configuration.Enums;
using ArturRios.UserManagement.IoC.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.Service.Provider;

public class ServiceProvider
{
    public ServiceProvider(IServiceCollection services, DataSource dataSource)
    {
        ConfigureDatabase(services, dataSource);
        ConfigureServices(services);
    }

    private static void ConfigureDatabase(IServiceCollection services, DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.Relational:
                services.AddRelationalContext();
                services.AddRelationalRepositories();
                break;
            case DataSource.ProtoBuf:
                services.AddProtoBufContext();
                break;
            case DataSource.NoSql:
            case DataSource.InMemory:
            case DataSource.Xml:
            case DataSource.Json:
            case DataSource.PlainText:
            default:
                throw new ArgumentOutOfRangeException(nameof(dataSource), dataSource, null);
        }
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddJwtTokenConfiguration();
        services.AddModelValidators();
        services.AddServices();
    }
}
