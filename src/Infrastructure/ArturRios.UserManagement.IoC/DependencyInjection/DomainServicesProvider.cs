using ArturRios.Common.Configuration.Enums;
using ArturRios.UserManagement.IoC.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class DomainServicesProvider
{
    public static void AddDomainServices(this IServiceCollection services, DomainServicesConfiguration configuration)
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
        
        services.AddJwtTokenConfiguration();
        services.AddModelValidators();
        services.AddServices();
    }
}