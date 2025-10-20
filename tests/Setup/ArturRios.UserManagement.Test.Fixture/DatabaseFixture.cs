using ArturRios.UserManagement.Data.Relational.Repositories;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Test.Mock;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Test.Fixture;

// ReSharper disable once ClassNeverInstantiated.Global
// Reason: this class is meant to be used as a fixture for xunit test and is not explicitly instantiated
public class DatabaseFixture : IDisposable
{
    private readonly RelationalDbContextFactory _dbContextFactory = new();
    private readonly List<int> _createdIds = [];
    private readonly Dictionary<int, string> _createdPasswords = [];

    public IEnumerable<User> CreateUsers(bool active = true, int quantity = 1)
    {
        List<User> createdUsers = [];

        using var dbContext = _dbContextFactory.CreateDbContext();

        for (var i = 0; i < quantity; i++)
        {
            var userMock = UserMock.New.WithNoId().WithRole(Roles.Test);

            if (!active)
            {
                userMock.Inactive();
            }

            var user = userMock.Generate();

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var createdId = user.Id;

            _createdIds.Add(createdId);

            _createdPasswords.Add(createdId, userMock.MockPassword);
            createdUsers.Add(user);
        }

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }

        return createdUsers;
    }

    public string GetPassword(int id)
    {
        return _createdPasswords[id];
    }

    public int GetUserNextId()
    {
        var maxId = GetUserMaxId();

        return maxId + 1 ?? 0;
    }

    public IEnumerable<User> GetAllUsers()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Users.AsNoTracking().ToList();
    }

    public User? GetUserById(int id)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
    }

    public void DeleteUsers(IEnumerable<int> ids)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var entities = dbContext.Users.Where(e => ids.Contains(e.Id)).ToList();

        dbContext.Users.RemoveRange(entities);

        dbContext.SaveChanges();
    }

    public void Dispose()
    {
        DeleteUsers(_createdIds);
        GC.SuppressFinalize(this);
    }

    private int? GetUserMaxId()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        return dbContext.Users.OrderByDescending(u => u.Id).FirstOrDefault()?.Id;
    }
}
