using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Management.User.Data.Configuration;

namespace TechCraftsmen.Management.User.Test.Fixture;

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

        DotNetEnv.Env.Load(envFile);
        
        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        var options = new RelationalDbContextOptions { ConnectionString = connectionString! };
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        
        return new RelationalDbContext(loggerFactory, options);
    }
}
