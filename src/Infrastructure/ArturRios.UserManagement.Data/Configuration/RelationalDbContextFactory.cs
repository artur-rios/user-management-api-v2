using ArturRios.Common.Environment;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Data.Configuration;

public class RelationalDbContextFactory : IDesignTimeDbContextFactory<RelationalDbContext>
{
    public RelationalDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            "../../Application/ArturRios.UserManagement.WebApi"));

        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The base path '{basePath}' does not exist.");
        }

        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Local";
        var envFilePath = Path.Combine(basePath, "Environments", $".env.{environmentName.ToLower()}");

        DotNetEnv.Env.Load(File.Exists(envFilePath)
            ? envFilePath
            : Path.Combine(basePath, "Environments", $".env.{nameof(EnvironmentType.Local).ToLower()}"));

        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Database connection string is not configured in the .env file.");
        }

        var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        return new RelationalDbContext(loggerFactory, new RelationalDbContextOptions
        {
            ConnectionString = connectionString
        });
    }
}
