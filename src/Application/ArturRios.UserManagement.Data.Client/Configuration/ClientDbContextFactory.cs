using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Data.Client.Configuration;

public class ClientDbContextFactory : IDbContextFactory<ClientDbContext>
{
    public ClientDbContext CreateDbContext()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var envFolder = Path.Combine(basePath, "Environments");
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Local";
        var envFile = Path.Combine(envFolder, $".env.{environmentName.ToLower()}");

        if (!File.Exists(envFile))
        {
            throw new FileNotFoundException($".env file not found at expected location: {envFile}");
        }

        DotNetEnv.Env.Load(envFile);

        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured in the .env file.");
        }

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        return new ClientDbContext(loggerFactory, new ClientDbContextOptions
        {
            ConnectionString = connectionString
        });
    }
}
    