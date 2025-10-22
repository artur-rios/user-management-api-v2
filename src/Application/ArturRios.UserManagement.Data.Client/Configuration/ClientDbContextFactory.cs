using ArturRios.Common.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Data.Client.Configuration;

public class ClientDbContextFactory : IDbContextFactory<ClientDbContext>
{
    public ClientDbContext CreateDbContext()
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured in the environment variables");
        }

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        return new ClientDbContext(loggerFactory, new BaseDbContextOptions { ConnectionString = connectionString });
    }
}
