using ArturRios.UserManagement.Data.Relational.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.Services;

public class HealthService(IDbContextFactory<RelationalDbContext> dbContextFactory)
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();

    public async Task<HealthCheckOutput> GetHealthStatusAsync()
    {
        var healthStatus = new HealthCheckOutput
        {
            DatabaseConnectionHealthy = await IsDatabaseConnectionHealthyAsync(),
            InitialAdminUserCreated = await InitialAdminUserCreated(),
            DefaultRegularUserCreated = await DefaultRegularUserCreated(),
            DefaultTestUserCreated = await DefaultTestUserCreated()
        };

        healthStatus.Healthy = healthStatus is
        {
            DatabaseConnectionHealthy: true,
            InitialAdminUserCreated: true,
            DefaultRegularUserCreated: true,
            DefaultTestUserCreated: true
        };

        return healthStatus;
    }

    private async Task<bool> IsDatabaseConnectionHealthyAsync()
    {
        return await _dbContext.Database.CanConnectAsync();
    }

    private async Task<bool> InitialAdminUserCreated()
    {
        var masterUserEmail = Environment.GetEnvironmentVariable("MASTER_USER_EMAIL");

        if (string.IsNullOrWhiteSpace(masterUserEmail))
        {
            throw new ArgumentException(
                "Master user configuration is incomplete, cannot check if initial master user is created");
        }

        return await _dbContext.Users.AnyAsync(u => u.Email == masterUserEmail);
    }

    private async Task<bool> DefaultRegularUserCreated()
    {
        var regularUserEmail = Environment.GetEnvironmentVariable("REGULAR_USER_EMAIL");

        if (string.IsNullOrWhiteSpace(regularUserEmail))
        {
            throw new ArgumentException(
                "Regular user configuration is incomplete, cannot check if initial regular user is created");
        }

        return await _dbContext.Users.AnyAsync(u => u.Email == regularUserEmail);
    }

    private async Task<bool> DefaultTestUserCreated()
    {
        var testUserEmail = Environment.GetEnvironmentVariable("TEST_USER_EMAIL");

        if (string.IsNullOrWhiteSpace(testUserEmail))
        {
            throw new ArgumentException(
                "Test user configuration is incomplete, cannot check if initial test user is created");
        }

        return await _dbContext.Users.AnyAsync(u => u.Email == testUserEmail);
    }
}
