using ArturRios.UserManagement.Data.Configuration;
using ArturRios.UserManagement.Data.Repositories;
using ArturRios.UserManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.WebApi.DependencyInjection;

public static class RelationalDatabaseConfiguration
{
    public static void AddRelationalContext(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured. Check your .env file.");
        }

        services.AddSingleton(new RelationalDbContextOptions { ConnectionString = connectionString });

        services.AddDbContext<RelationalDbContext>(options =>
            options.UseNpgsql(connectionString), 
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddDbContextFactory<RelationalDbContext>();
        services.AddScoped<RelationalDbInitializer>();
    }

    public static void AddRelationalRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}