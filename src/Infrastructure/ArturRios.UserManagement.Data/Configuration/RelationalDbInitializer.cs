using ArturRios.UserManagement.Data.DataSeed;

namespace ArturRios.UserManagement.Data.Configuration;

public class RelationalDbInitializer(RelationalDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.EnsureCreatedAsync();
        
        await RoleSeeder.CreateRoles(context);
        await UserSeeder.CreateInitialUsers(context);
    }
}
