using ArturRios.UserManagement.Data.Relational.DataSeed;

namespace ArturRios.UserManagement.Data.Relational.Configuration;

public class RelationalDbInitializer(RelationalDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.EnsureCreatedAsync();

        await RoleSeeder.EnsureRolesCreated(context);
        await UserSeeder.EnsureInitialUsersCreated(context);
    }
}
