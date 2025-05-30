using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Management.User.Data.Configuration;

namespace TechCraftsmen.Management.User.Test.Fixture;

public class TestRepository(IDbContextFactory<RelationalDbContext> dbContextFactory)
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();

    public int GetUserNextId()
    {
        var maxId = GetUserMaxId();
        
        return maxId + 1 ?? 0;
    }

    private int? GetUserMaxId()
    {
        return _dbContext.Users.OrderByDescending(u => u.Id).FirstOrDefault()?.Id;
    }
}
