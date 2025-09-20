using ArturRios.Common.Extensions;
using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.DataSeed;

public static class RoleSeeder
{
    public static async Task CreateRoles(RelationalDbContext context)
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
