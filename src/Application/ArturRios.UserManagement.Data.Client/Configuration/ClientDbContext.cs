using ArturRios.Common.Data.Configuration;
using ArturRios.UserManagement.Data.Client.DatabaseMaps;
using ArturRios.UserManagement.Data.Client.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArturRios.UserManagement.Data.Client.Configuration;

public class ClientDbContext(ILoggerFactory loggerFactory, BaseDbContextOptions options) : DbContext
{
    private const string Schema = "user_management";
    
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
        
        modelBuilder.Entity<User>().Configure();
    }
}
