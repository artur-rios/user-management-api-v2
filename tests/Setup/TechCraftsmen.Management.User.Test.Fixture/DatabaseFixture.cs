using TechCraftsmen.Management.User.Data.Repositories;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.Test.Fixture;

// ReSharper disable once ClassNeverInstantiated.Global
// Reason: this class is meant to be used as a fixture for xunit test and is not explicitly instantiated
public class DatabaseFixture : IDisposable
{
    public Domain.Aggregates.User TestUser { get; }
    public string TestPassword { get; }
    
    private readonly UserRepository _userRepository;

    private readonly int _userId;

    public DatabaseFixture()
    {
        var dbContextFactory = new RelationalDbContextFactory();
        
        var testRepository = new TestRepository(dbContextFactory);
        _userRepository = new UserRepository(dbContextFactory);

        var testUserId = testRepository.GetUserNextId();
        
        var userMock = UserMock.New.WithId(testUserId).WithRole(Roles.Regular);

        TestUser = userMock.Generate();
        TestPassword = userMock.MockPassword;

        _userId = _userRepository.Create(TestUser);
    }

    public void Dispose()
    {
        _userRepository.Delete(_userId);
        GC.SuppressFinalize(this);
    }
}
