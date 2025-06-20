using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Domain.Interfaces;

namespace TechCraftsmen.Management.User.Data.Repositories;

public class UserRepository(IDbContextFactory<RelationalDbContext> dbContextFactory) : IUserRepository
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

    public IQueryable<Domain.Aggregates.User> GetByMultiFilter(DataFilter filter, bool track = false)
    {
        if (filter is not UserMultiFilter multiFilter)
        {
            throw new ArgumentException("Invalid filter");
        }

        var names = multiFilter.Names?.ToList();
        var emails = multiFilter.Emails?.ToList();
        var roleIds = multiFilter.RoleIds?.ToList();
        var creationDates = multiFilter.CreationDates?.ToList();

        var query = _dbContext.Set<Domain.Aggregates.User>().AsQueryable();

        if (names.IsNotEmpty())
        {
            query = query.Where(e => names!.Contains(e.Name));
        }

        if (emails.IsNotEmpty())
        {
            query = query.Where(e => emails!.Contains(e.Email));
        }

        if (roleIds.IsNotEmpty())
        {
            query = query.Where(e => roleIds!.Contains(e.RoleId));
        }

        if (creationDates.IsNotEmpty())
        {
            query = query.Where(e => creationDates!.Contains(e.CreatedAt));
        }

        return track ? query : query.AsNoTracking();
    }

    public void MultiDelete(IEnumerable<int> ids)
    {
        var users = _dbContext.Users.Where(u => ids.Contains(u.Id)).ToList();
        _dbContext.Users.RemoveRange(users);
        _dbContext.SaveChanges();
    }
}
