using ArturRios.Common.Configuration.Enums;
using ArturRios.UserManagement.IoC.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.Service.Provider;

public class ServiceProvider
{
    public ServiceProvider(IServiceCollection services, ServiceConfiguration configuration)
    {
        ConfigureDatabase(services, configuration);
        ConfigureServices(services);
    }

    private static void ConfigureDatabase(IServiceCollection services, ServiceConfiguration configuration)
    {
        switch (configuration.DataSource)
        {
            case DataSource.Relational:
                services.AddRelationalContext();
                services.AddRelationalRepositories();
                break;
            case DataSource.NoSql:
            case DataSource.InMemory:
            default:
                throw new ArgumentOutOfRangeException(nameof(configuration), configuration.DataSource, null);
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddJwtTokenConfiguration();
        services.AddModelValidators();
        services.AddServices();
    }
}