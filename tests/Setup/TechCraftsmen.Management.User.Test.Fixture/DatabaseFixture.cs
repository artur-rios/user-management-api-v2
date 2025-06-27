using Microsoft.EntityFrameworkCore;
using TechCraftsmen.Management.User.Data.Repositories;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Dto.Mapping;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.Test.Fixture;

// ReSharper disable once ClassNeverInstantiated.Global
// Reason: this class is meant to be used as a fixture for xunit test and is not explicitly instantiated
public class DatabaseFixture : IDisposable
{
    private readonly RelationalDbContextFactory _dbContextFactory;
    private readonly UserRepository _userRepository;

    private readonly List<int> _createdIds = [];
    
    public DatabaseFixture()
    {
        _dbContextFactory = new RelationalDbContextFactory();
        _userRepository = new UserRepository(_dbContextFactory);
    }

    public IEnumerable<UserDto> CreateUsers(bool active = true, int quantity = 1)
    {
        List<UserDto> createdUsers = [];
        
        using var dbContext = _dbContextFactory.CreateDbContext();

        for (var i = 0; i < quantity; i++)
        {
            var userMock = UserMock.New.WithNoId().WithRole(Roles.Test);
            
            if (!active)
            {
                userMock.Inactive();
            }
            
            var user = userMock.Generate();
            var userDto = user.ToDto();
            userDto.Password = userMock.MockPassword;

            var createdId = _userRepository.Create(user);
            
            userDto.Id = createdId;
            _createdIds.Add(createdId);
            
            createdUsers.Add(userDto);
        }
        
        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }

        return createdUsers;
    }
    
    public IEnumerable<UserDto> GetUsersByMultiFilter(UserMultiFilter filter)
    {
        return _userRepository.GetByMultiFilter(filter).Select(user => user.ToDto());
    }
    
    public UserDto? GetUserById(int id)
    {
        return _userRepository.GetById(id)?.ToDto();
    }

    public void Dispose()
    {
        _userRepository.MultiDelete(_createdIds);
        GC.SuppressFinalize(this);
    }
}
