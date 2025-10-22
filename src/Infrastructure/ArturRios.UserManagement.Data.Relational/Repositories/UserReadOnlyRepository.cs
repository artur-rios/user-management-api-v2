using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.Repositories;

public class UserReadOnlyRepository(IDbContextFactory<RelationalDbContext> dbContextFactory) : IUserReadOnlyRepository
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();

    public IQueryable<User> GetAll() => _dbContext.Users.AsNoTracking();

    public User? GetById(int id) => _dbContext.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);
}
