using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Domain.Aggregates;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Data.DataSeed;

public static class RoleSeeder
{
    public static async Task EnsureRolesExist(RelationalDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            var roles = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(role => new Role
                {
                    Id = (int)role,
                    Name = role.ToString(),
                    Description = role.GetDescription()!
                })
                .ToList();

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }
    }
}
