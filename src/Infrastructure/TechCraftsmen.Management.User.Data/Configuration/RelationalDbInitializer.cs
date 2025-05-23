using TechCraftsmen.Management.User.Data.DataSeed;

namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbInitializer(RelationalDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.EnsureCreatedAsync();
        
        await RoleSeeder.EnsureRolesExist(context);
        await UserSeeder.EnsureMasterUserExists(context);
    }
}
