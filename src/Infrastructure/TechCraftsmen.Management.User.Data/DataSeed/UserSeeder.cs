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

        var hasMasterUserData = !(string.IsNullOrWhiteSpace(masterUserName) ||
                                string.IsNullOrWhiteSpace(masterUserEmail) ||
                                string.IsNullOrWhiteSpace(masterUserPassword));

        if (!hasMasterUserData)
        {
            throw new ArgumentException("Master user configuration is incomplete. Cannot create master user.");
        }

        var masterPasswordHash = Hash.NewFromText(masterUserPassword!);

        var masterUser = new Domain.Aggregates.User
        {
            Name = masterUserName!,
            Email = masterUserEmail!,
            Password = masterPasswordHash.Value,
            Salt = masterPasswordHash.Salt,
            RoleId = (int)Roles.Admin,
            CreatedAt = DateTime.UtcNow,
            Active = true
        };

        context.Users.Add(masterUser);
        
        var regularUserName = Environment.GetEnvironmentVariable("REGULAR_USER_NAME");
        var regularUserEmail = Environment.GetEnvironmentVariable("REGULAR_USER_EMAIL");
        var regularUserPassword = Environment.GetEnvironmentVariable("REGULAR_USER_PASSWORD");
        
        var hasRegularUserData = !(string.IsNullOrWhiteSpace(regularUserName) ||
                                string.IsNullOrWhiteSpace(regularUserEmail) ||
                                string.IsNullOrWhiteSpace(regularUserPassword));
        
        if (hasRegularUserData)
        {
            var regularPasswordHash = Hash.NewFromText(regularUserPassword!);
            
            var testUser = new Domain.Aggregates.User
            {
                Name = regularUserName!,
                Email = regularUserEmail!,
                Password = regularPasswordHash.Value,
                Salt = regularPasswordHash.Salt,
                RoleId = (int)Roles.Regular,
                CreatedAt = DateTime.UtcNow,
                Active = true
            };

            context.Users.Add(testUser);
        }
        
        var testUserName = Environment.GetEnvironmentVariable("TEST_USER_NAME");
        var testUserEmail = Environment.GetEnvironmentVariable("TEST_USER_EMAIL");
        var testUserPassword = Environment.GetEnvironmentVariable("TEST_USER_PASSWORD");
        
        var hasTestUserData = !(string.IsNullOrWhiteSpace(testUserName) ||
                                string.IsNullOrWhiteSpace(testUserEmail) ||
                                string.IsNullOrWhiteSpace(testUserPassword));
        
        if (hasTestUserData)
        {
            var testPasswordHash = Hash.NewFromText(testUserPassword!);
            
            var testUser = new Domain.Aggregates.User
            {
                Name = testUserName!,
                Email = testUserEmail!,
                Password = testPasswordHash.Value,
                Salt = testPasswordHash.Salt,
                RoleId = (int)Roles.Test,
                CreatedAt = DateTime.UtcNow,
                Active = true
            };

            context.Users.Add(testUser);
        }
        
        await context.SaveChangesAsync();
    }
}
