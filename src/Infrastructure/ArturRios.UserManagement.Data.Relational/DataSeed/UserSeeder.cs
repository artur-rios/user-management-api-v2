using ArturRios.Common.Util.Hashing;
using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;

namespace ArturRios.UserManagement.Data.Relational.DataSeed;

public static class UserSeeder
{
    public static async Task CreateInitialUsers(RelationalDbContext context)
    {
        if (!UserWithRoleExists(context, Roles.Admin))
        {
            var masterUserName = Environment.GetEnvironmentVariable("MASTER_USER_NAME");
            var masterUserEmail = Environment.GetEnvironmentVariable("MASTER_USER_EMAIL");
            var masterUserPassword = Environment.GetEnvironmentVariable("MASTER_USER_PASSWORD");

            if (!HasUserData(masterUserName, masterUserEmail, masterUserPassword))
            {
                throw new ArgumentException("Master user configuration is incomplete, cannot create master user");
            }

            CreateUser(context, masterUserName!, masterUserEmail!, masterUserPassword!, Roles.Admin);
        }

        if (!UserWithRoleExists(context, Roles.Regular))
        {
            var regularUserName = Environment.GetEnvironmentVariable("REGULAR_USER_NAME");
            var regularUserEmail = Environment.GetEnvironmentVariable("REGULAR_USER_EMAIL");
            var regularUserPassword = Environment.GetEnvironmentVariable("REGULAR_USER_PASSWORD");

            if (HasUserData(regularUserName, regularUserEmail, regularUserPassword))
            {
                CreateUser(context, regularUserName!, regularUserEmail!, regularUserPassword!, Roles.Regular);
            }
        }

        if (!UserWithRoleExists(context, Roles.Test))
        {
            var testUserName = Environment.GetEnvironmentVariable("TEST_USER_NAME");
            var testUserEmail = Environment.GetEnvironmentVariable("TEST_USER_EMAIL");
            var testUserPassword = Environment.GetEnvironmentVariable("TEST_USER_PASSWORD");

            if (HasUserData(testUserName, testUserEmail, testUserPassword))
            {
                CreateUser(context, testUserName!, testUserEmail!, testUserPassword!, Roles.Test);
            }
        }

        await context.SaveChangesAsync();
    }

    private static void CreateUser(RelationalDbContext context, string name, string email, string password, Roles role)
    {
        var passwordHash = Hash.NewFromText(password);

        var user = new User(name, email, (int)role, DateTime.UtcNow);
        user.SetPassword(passwordHash.Value, passwordHash.Salt);

        context.Users.Add(user);
    }

    private static bool HasUserData(string? username, string? email, string? password) =>
        !(string.IsNullOrWhiteSpace(username) ||
          string.IsNullOrWhiteSpace(email) ||
          string.IsNullOrWhiteSpace(password));

    private static bool UserWithRoleExists(RelationalDbContext context, Roles role) =>
        context.Users.Any(user => user.RoleId == (int)role);
}
