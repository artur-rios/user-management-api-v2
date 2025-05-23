using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Data.Repositories;

namespace TechCraftsmen.Management.User.Test.Fixture;

public class DatabaseFixture : IDisposable
{
    private readonly UserRepository _userRepository;
    private Domain.Aggregates.User TestUser { get; }
    private readonly int _userId;

    public DatabaseFixture()
    {
        var dbContextFactory = CreateDbContextFactory();
        _userRepository = new UserRepository(dbContextFactory);

        TestUser = new Domain.Aggregates.User
        {
            Name = "Test User",
            Email = "testuser@example.com",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            Active = true
        };

        _userId = _userRepository.Create(TestUser);
    }

    private static PooledDbContextFactory<RelationalDbContext> CreateDbContextFactory()
    {
        var envFile = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Application", 
            "TechCraftsmen.Management.User.WebApi", "Environments", $".env.{nameof(EnvironmentType.Local).ToLower()}");
        
        if (!File.Exists(envFile))
        {
            envFile = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Application", 
                "TechCraftsmen.Management.User.WebApi", "Environments", ".env");
        }

        DotNetEnv.Env.Load(envFile);
        
        var connectionString = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING");

        var optionsBuilder = new DbContextOptionsBuilder<RelationalDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new PooledDbContextFactory<RelationalDbContext>(optionsBuilder.Options);
    }

    public void Dispose()
    {
        _userRepository.Delete(_userId);
        GC.SuppressFinalize(this);
    }
}
