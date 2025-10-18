using ArturRios.Common.Data.Configuration;
using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Data.Relational.Repositories;
using ArturRios.UserManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class RelationalDatabaseProvider
{
    public static void AddRelationalContext(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured. Check your .env file.");
        }

        services.AddSingleton(new BaseDbContextOptions { ConnectionString = connectionString });

        services.AddDbContext<RelationalDbContext>(options =>
            options.UseNpgsql(connectionString),
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddDbContextFactory<RelationalDbContext>();
        services.AddScoped<RelationalDbInitializer>();
    }

    public static void AddRelationalRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
        services.AddScoped<IUserRangeRepository, UserRangeRepository>();
    }
}
