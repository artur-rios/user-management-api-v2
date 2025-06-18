using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Domain.Filters;

namespace TechCraftsmen.Management.User.Data.Repositories;

public class UserRepository(IDbContextFactory<RelationalDbContext> dbContextFactory) : ICrudRepository<Domain.Aggregates.User>
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();
    public int Create(Domain.Aggregates.User entity)
    {
        _dbContext.Users.Add(entity);
        _dbContext.SaveChanges();

        return entity.Id;
    }

    public IQueryable<Domain.Aggregates.User> GetByFilter(DataFilter filter, bool track = false)
    {
        if (filter is not UserFilter userFilter)
        {
            throw new ArgumentException("Invalid filter");
        }
        
        var query = _dbContext.Set<Domain.Aggregates.User>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(userFilter.Name))
        {
            query = query.Where(e => e.Name == userFilter.Name);
        }
        
        if (!string.IsNullOrWhiteSpace(userFilter.Email))
        {
            query = query.Where(e => e.Email == userFilter.Email);
        }
        
        if (userFilter.RoleId.HasValue)
        {
            query = query.Where(e => e.RoleId == userFilter.RoleId.Value);
        }
        
        if (userFilter.CreatedAt.HasValue)
        {
            query = query.Where(e => e.CreatedAt == userFilter.CreatedAt.Value);
        }
        
        if (userFilter.Active.HasValue)
        {
            query = query.Where(e => e.Active == userFilter.Active.Value);
        }
        
        return track ? query : query.AsNoTracking();
    }

    public Domain.Aggregates.User? GetById(int id, bool track = false)
    {
        return track ? _dbContext.Users.Find(id) : _dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Id == id);
    }

    public void Update(Domain.Aggregates.User entity)
    {
        _dbContext.Update(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = _dbContext.Users.Find(id);
        _dbContext.Remove(user!);
        _dbContext.SaveChanges();
    }
}
