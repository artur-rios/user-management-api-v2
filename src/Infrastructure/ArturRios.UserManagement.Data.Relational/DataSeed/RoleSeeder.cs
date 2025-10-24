using ArturRios.Common.Extensions;
using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.DataSeed;

public static class RoleSeeder
{
    public static async Task EnsureRolesCreated(RelationalDbContext context)
    {
        var rolesFromEnum = Enum.GetValues(typeof(Roles))
            .Cast<Roles>()
            .ToList();

        var existingRoles = await context.Roles.AsNoTracking().ToListAsync();

        await using var tx = await context.Database.BeginTransactionAsync();
        try
        {
            foreach (var roleEnum in rolesFromEnum)
            {
                var desiredId = (int)roleEnum;
                var name = roleEnum.ToString();
                var description = roleEnum.GetDescription() ?? string.Empty;

                var existingById = existingRoles.FirstOrDefault(r => r.Id == desiredId);

                if (existingById != null)
                {
                    var tracked = await context.Roles.FindAsync(desiredId);

                    if (tracked == null)
                    {
                        var newRole = new Role { Id = desiredId, Name = name, Description = description };
                        context.Roles.Add(newRole);

                        await context.SaveChangesAsync();

                        continue;
                    }

                    var needsUpdate = tracked.Name != name || tracked.Description != description;

                    if (needsUpdate)
                    {
                        var updated = new Role { Id = desiredId, Name = name, Description = description };

                        context.Entry(tracked).CurrentValues.SetValues(updated);

                        await context.SaveChangesAsync();
                    }

                    continue;
                }

                var existingByName = existingRoles.FirstOrDefault(r => r.Name == name);

                if (existingByName != null)
                {
                    var oldId = existingByName.Id;

                    var newRole = new Role { Id = desiredId, Name = name, Description = description };

                    context.Roles.Add(newRole);

                    await context.SaveChangesAsync();

                    var sql = $"UPDATE \"{Constants.Schema}\".\"Users\" SET \"RoleId\" = {desiredId} WHERE \"RoleId\" = {oldId}";

                    await context.Database.ExecuteSqlRawAsync(sql);

                    var oldRole = await context.Roles.FindAsync(oldId);

                    if (oldRole != null)
                    {
                        context.Roles.Remove(oldRole);

                        await context.SaveChangesAsync();
                    }

                    existingRoles = await context.Roles.AsNoTracking().ToListAsync();

                    continue;
                }

                var roleToAdd = new Role { Id = desiredId, Name = name, Description = description };

                context.Roles.Add(roleToAdd);

                await context.SaveChangesAsync();

                existingRoles.Add(roleToAdd);
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();

            throw;
        }
    }
}
