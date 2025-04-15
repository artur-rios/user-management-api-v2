using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TechCraftsmen.Management.User.Data.DataSeed;
using TechCraftsmen.Management.User.Data.EntityMaps;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbContext(ILoggerFactory loggerFactory, IOptions<RelationalDbContextOptions> options) : DbContext
{
    private readonly RelationalDbContextOptions _options = options.Value;
    
    private const string Schema = "user_management";
    
    public DbSet<Role> Roles { get; init; }
    public DbSet<Domain.Aggregates.User> Users { get; init; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_options.RelationalDatabase);

        optionsBuilder.UseSeeding((context, _) =>
        {
            RoleSeeder.EnsureRolesExist(context);
        });

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
