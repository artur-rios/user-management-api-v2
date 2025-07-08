using ArturRios.UserManagement.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Test.Fixture;

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
