using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Data.Relational.Repositories;

public class UserRepository(IDbContextFactory<RelationalDbContext> dbContextFactory) : IUserRepository
{
    private readonly RelationalDbContext _dbContext = dbContextFactory.CreateDbContext();

    public int Create(User entity)
    {
        _dbContext.Users.Add(entity);

        _dbContext.SaveChanges();

        return entity.Id;
    }

    public IQueryable<User> GetAll() => _dbContext.Users;

    public User? GetById(int id) => _dbContext.Users.Find(id);

    public User Update(User entity)
    {
        _dbContext.Update(entity);

        _dbContext.SaveChanges();

        return entity;
    }

    public int Delete(User entity)
    {
        _dbContext.Users.Remove(entity);

        _dbContext.SaveChanges();

        return entity.Id;
    }
}
