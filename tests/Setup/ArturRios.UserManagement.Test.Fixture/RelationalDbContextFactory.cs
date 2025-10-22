using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Data.Configuration;
using ArturRios.UserManagement.Data.Relational.Configuration;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Test.Fixture;

public class RelationalDbContextFactory : IDbContextFactory<RelationalDbContext>
{
    public RelationalDbContext CreateDbContext()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var envFolder = Path.Combine(basePath, "Environments");
        var envFile = Path.Combine(envFolder, $".env.{nameof(EnvironmentType.Local).ToLower()}");

        if (!File.Exists(envFile))
        {
            throw new FileNotFoundException($".env file not found at expected location: {envFile}");
        }

        Env.Load(envFile);

        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        return new RelationalDbContext(loggerFactory,
            new BaseDbContextOptions { ConnectionString = connectionString! });
    }
}
