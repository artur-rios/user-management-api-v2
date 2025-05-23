using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Core.Util.Hashing;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Data.DataSeed;

public static class UserSeeder
{
    public static async Task EnsureMasterUserExists(RelationalDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var masterUserName = Environment.GetEnvironmentVariable("MASTER_USER_NAME");
        var masterUserEmail = Environment.GetEnvironmentVariable("MASTER_USER_EMAIL");
        var masterUserPassword = Environment.GetEnvironmentVariable("MASTER_USER_PASSWORD");

        if (string.IsNullOrWhiteSpace(masterUserName) ||
            string.IsNullOrWhiteSpace(masterUserEmail) ||
            string.IsNullOrWhiteSpace(masterUserPassword))
        {
            throw new ArgumentException("Master user configuration is incomplete. Cannot create master user.");
        }

        var hash = Hash.NewFromText(masterUserPassword);

        var masterUser = new Domain.Aggregates.User
        {
            Name = masterUserName,
            Email = masterUserEmail,
            Password = hash.Value,
            Salt = hash.Salt,
            RoleId = (int)Roles.Admin,
            CreatedAt = DateTime.UtcNow,
            Active = true
        };

        context.Users.Add(masterUser);
        await context.SaveChangesAsync();
    }
}
