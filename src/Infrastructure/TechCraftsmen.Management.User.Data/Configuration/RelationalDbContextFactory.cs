using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbContextFactory : IDesignTimeDbContextFactory<RelationalDbContext>
{
    public RelationalDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../Application/TechCraftsmen.Management.User.WebApi"));
        
        if (!Directory.Exists(basePath))
        {
            throw new DirectoryNotFoundException($"The base path '{basePath}' does not exist.");
        }
        
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Local";
        var appSettings = $"appsettings.api.{environmentName}.json";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile(appSettings, optional: false)
            .Build();
        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        
        var options = Options.Create(new RelationalDbContextOptions
        {
            RelationalDatabase = configuration.GetConnectionString("RelationalDatabase")!
        });
        
        return new RelationalDbContext(loggerFactory, options);
    }
}