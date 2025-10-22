using ArturRios.UserManagement.Data.Client.Configuration;
using ArturRios.UserManagement.Data.Client.Model;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Client.Repositories;

public class UserRepository(IDbContextFactory<ClientDbContext> dbContextFactory)
{
    private readonly ClientDbContext _dbContext = dbContextFactory.CreateDbContext();

    public User? GetById(int id) => _dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Id == id);

    public IQueryable<User> GetAll() => _dbContext.Users.AsNoTracking();
}
