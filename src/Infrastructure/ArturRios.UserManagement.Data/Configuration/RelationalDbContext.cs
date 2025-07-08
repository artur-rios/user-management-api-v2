using ArturRios.UserManagement.Data.EntityMaps;
using ArturRios.UserManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Data.Configuration;

public class RelationalDbContext(ILoggerFactory loggerFactory, RelationalDbContextOptions options) : DbContext
{
    private const string Schema = "user_management";

    public DbSet<Role> Roles { get; init; }
    public DbSet<User> Users { get; init; }

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
        modelBuilder.Entity<User>().Configure();
    }
}
