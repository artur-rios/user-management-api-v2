using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbContext : DbContext
{
    private const string Schema = "UserManagement";
    
    public DbSet<Role> Roles { get; init; }
    public DbSet<Domain.Aggregates.User> Users { get; init; }
}
