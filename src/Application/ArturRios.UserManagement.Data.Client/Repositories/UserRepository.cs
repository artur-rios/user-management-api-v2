using ArturRios.Common.Data;
using ArturRios.UserManagement.Data.Client.Configuration;
using ArturRios.UserManagement.Data.Client.Filters;
using ArturRios.UserManagement.Data.Client.Model;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Client.Repositories;

public class UserRepository(IDbContextFactory<ClientDbContext> dbContextFactory)
{
    private readonly ClientDbContext _dbContext = dbContextFactory.CreateDbContext();
    
    public User? GetById(int id, bool track = false)
    {
        return track ? _dbContext.Users.Find(id) : _dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Id == id);
    }
    
    public IQueryable<User> GetByFilter(DataFilter filter, bool track = false)
    {
        if (filter is not UserFilter userFilter)
        {
            throw new ArgumentException("Invalid filter");
        }
        
        var query = _dbContext.Set<User>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(userFilter.Name))
        {
            query = query.Where(e => e.Name == userFilter.Name);
        }
        
        if (!string.IsNullOrWhiteSpace(userFilter.Email))
        {
            query = query.Where(e => e.Email == userFilter.Email);
        }
        
        // if (userFilter.RoleId.HasValue)
        // {
        //     query = query.Where(e => e.RoleId == userFilter.RoleId.Value);
        // }
        
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
}