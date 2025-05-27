using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechCraftsmen.Management.User.Data.EntityMaps;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbContext(ILoggerFactory loggerFactory, RelationalDbContextOptions options) : DbContext
{
    private const string Schema = "user_management";
    
    public DbSet<Role> Roles { get; init; }
    public DbSet<Domain.Aggregates.User> Users { get; init; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options.ConnectionString);
        
        optionsBuilder
            .UseLoggerFactory(loggerFactory)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        
        modelBuilder.Entity<Role>().Configure();
        modelBuilder.Entity<Domain.Aggregates.User>().Configure();
    }
}
