using TechCraftsmen.Core.Data;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Data.Repositories;

namespace TechCraftsmen.Management.User.WebApi.DependencyInjection;

public static class RelationalDatabaseConfiguration
{
    public static void AddRelationalContext(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<RelationalDbContextOptions>(configuration);

        services.AddDbContext<RelationalDbContext>(optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<RelationalDbContext>();
    }

    public static void AddRelationalRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICrudRepository<Domain.Aggregates.User>, UserRepository>();
    }
}
