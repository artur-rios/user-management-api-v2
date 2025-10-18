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
    private readonly RelationalDbContextFactory _dbContextFactory;
    private readonly TestRepository _testRepository;
    private readonly UserRepository _userRepository;
    private readonly UserRangeRepository _userRangeRepository;

    private readonly List<int> _createdIds = [];
    private readonly Dictionary<int, string> _createdPasswords = [];

    public DatabaseFixture()
    {
        _dbContextFactory = new RelationalDbContextFactory();
        _testRepository = new TestRepository(_dbContextFactory);
        _userRepository = new UserRepository(_dbContextFactory);
        _userRangeRepository = new UserRangeRepository(_dbContextFactory);
    }

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

            var createdId = _userRepository.Create(user);

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
        return _testRepository.GetUserNextId();
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetAll();
    }

    public User? GetUserById(int id)
    {
        return _userRepository.GetById(id);
    }

    public void DeleteUsers(IEnumerable<int> ids)
    {
        _userRangeRepository.DeleteRange(ids.ToList());
    }

    public void Dispose()
    {
        _userRangeRepository.DeleteRange(_createdIds);
        GC.SuppressFinalize(this);
    }
}
