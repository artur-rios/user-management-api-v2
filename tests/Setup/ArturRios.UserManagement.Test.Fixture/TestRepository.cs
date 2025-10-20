using ArturRios.UserManagement.Data.Relational.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Test.Fixture;

public class TestRepository(IDbContextFactory<RelationalDbContext> dbContextFactory)
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();


}
