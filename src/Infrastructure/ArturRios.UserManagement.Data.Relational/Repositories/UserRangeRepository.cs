using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.Repositories;

public class UserRangeRepository(IDbContextFactory<RelationalDbContext> dbContextFactory) : IUserRangeRepository
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();

    public IEnumerable<User> UpdateRange(List<User> entities)
    {
        _dbContext.Users.UpdateRange(entities);

        _dbContext.SaveChanges();

        return entities;
    }

    public IEnumerable<int> DeleteRange(List<int> ids)
    {
        var entities = _dbContext.Users.Where(e => ids.Contains(e.Id)).ToList();

        _dbContext.Users.RemoveRange(entities);

        _dbContext.SaveChanges();

        return entities.Select(e => e.Id);
    }
}
