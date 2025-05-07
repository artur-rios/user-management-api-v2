using Bogus;
using Microsoft.AspNetCore.Http;
using Moq;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Dto.Validation;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.Services.Tests;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Faker _faker = new();

    private const int ExistingId = 1;
    private const int NonexistentId = 2;
    private const int InactiveId = 3;
    private const string ActiveEmail = "active@mail.com";
    private const string InactiveEmail = "inactive@mail.com";

    public UserServiceTests()
    {
        var userRepository = new Mock<ICrudRepository<Domain.Aggregates.User>>();
        var httpContextAccessor = new Mock<HttpContextAccessor>();
        var userDtoValidator = new UserDtoValidator();

        userRepository.Setup(repo => repo.Create(It.IsAny<Domain.Aggregates.User>()))
            .Returns(() => _faker.Random.Int(1, 1000));

        userRepository.Setup(repo => repo.GetById(It.IsAny<int>()))
            .Returns((int id) => id switch
            {
                ExistingId => UserMock.New.WithId(ExistingId).Generate(),
                InactiveId => UserMock.New.WithId(InactiveId).WithEmail(InactiveEmail).Inactive().Generate(),
                _ => null
            });

        userRepository.Setup(repo => repo.GetByFilter(It.IsAny<UserFilter>()))
            .Returns((UserFilter userFilter) => userFilter.Email == ActiveEmail
                ? new List<Domain.Aggregates.User>
                {
                    UserMock.New.WithEmail(ActiveEmail).Generate()
                }.AsQueryable()
                : new List<Domain.Aggregates.User>().AsQueryable());

        userRepository.Setup(repo => repo.Update(It.IsAny<Domain.Aggregates.User>()));

        userRepository.Setup(repo => repo.Delete(It.IsAny<int>()));

        httpContextAccessor.Object.HttpContext = new DefaultHttpContext();
        httpContextAccessor.Object.HttpContext!.Items = new Dictionary<object, object?>
            { { "User", new AuthenticatedUserMock(Roles.Regular).Generate() } };

        _userService = new UserService(userRepository.Object, httpContextAccessor.Object, userDtoValidator);
    }

    [Unit]
    public void Should_ReturnValidationError_When_UserDtoIsInvalid()
    {
        var dto = UserMock.New.WithEmail("").Generate().ToDtoWithPassword();

        var result = _userService.CreateUser(dto);

        Assert.Equal(0, result.Data);
        Assert.Equal("Email should be valid", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_NotCreateUser_When_EmailIsAlreadyRegistered()
    {
        var dto = UserMock.New.WithEmail(ActiveEmail).Generate().ToDtoWithPassword();
        
        var result = _userService.CreateUser(dto);
        
        Assert.Equal(0, result.Data);
        Assert.Equal("E-mail already registered", result.Messages.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_NotCreateAdminUser_When_AuthenticatedUserIsNotAdmin()
    {
        var dto = UserMock.New.WithRole(Roles.Admin).Generate().ToDtoWithPassword();
        
        var result = _userService.CreateUser(dto);
        
        Assert.Equal(0, result.Data);
        Assert.Equal($"Only admins can register a user with {Roles.Admin.ToString()} role", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_CreateUser_And_ReturnId_ForValidDto()
    {
        var dto = UserMock.New.WithRole(Roles.Regular).Generate().ToDtoWithPassword();
        
        var result = _userService.CreateUser(dto);
        
        Assert.True(result.Data > 0);
        Assert.Equal("User created with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [Unit]
    public void Should_ReturnNull_When_IdNotOnDatabase()
    {
        var result = _userService.GetUserById(NonexistentId);
        
        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_ReturnUser_When_IdIsOnDatabase()
    {
        var result = _userService.GetUserById(ExistingId);
        
        Assert.NotNull(result.Data);
        Assert.Equal("User found", result.Messages.First());
        Assert.True(result.Success);
    }

    [Unit]
    public void Should_ReturnEmptyList_When_FilterDoesNotMatchAnyUser()
    {
        var filter = new UserFilter{ Email = InactiveEmail };
        var result = _userService.GetUsersByFilter(filter);
        
        Assert.Empty(result.Data!);
        Assert.Equal("No users found for the given filter", result.Messages.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_ReturnListOfUsers_When_FilterMatchesUsers()
    {
        var filter = new UserFilter{ Email = ActiveEmail };
        var result = _userService.GetUsersByFilter(filter);
        
        Assert.NotEmpty(result.Data!);
        Assert.Equal("Users found", result.Messages.First());
        Assert.True(result.Success);
    }
    
    [Unit]
    public void Should_NotUpdateUser_When_IdIsNotOnDatabase()
    {
        var dto = UserMock.New.WithId(NonexistentId).Generate().ToDtoWithPassword();
        
        var result = _userService.UpdateUser(dto);
        
        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Messages.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_NotUpdateUser_When_UserIsInactive()
    {
        var dto = UserMock.New.WithId(InactiveId).Inactive().Generate().ToDtoWithPassword();
        
        var result = _userService.UpdateUser(dto);
        
        Assert.Null(result.Data);
        Assert.Equal("Can't update inactive user", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_UpdateUser()
    {
        var dto = UserMock.New.WithId(ExistingId).Generate().ToDtoWithPassword();
        
        var result = _userService.UpdateUser(dto);
        
        Assert.NotNull(result.Data);
        Assert.Equal("User updated with success", result.Messages.First());
        Assert.True(result.Success);
    }
    
    [Unit]
    public void Should_NotActivateUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.ActivateUser(NonexistentId);
        
        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_NotActivateUser_When_UserIsAlreadyActive()
    {
        var result = _userService.ActivateUser(ExistingId);
        
        Assert.Equal("User already active", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_ActivateUser()
    {
        var result = _userService.ActivateUser(InactiveId);
        
        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }
    
    [Unit]
    public void Should_NotDeactivateUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.DeactivateUser(NonexistentId);
        
        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_NotDeactivateUser_When_UserIsAlreadyInactive()
    {
        var result = _userService.DeactivateUser(InactiveId);
        
        Assert.Equal("User already inactive", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_DeactivateUser()
    {
        var result = _userService.DeactivateUser(ExistingId);
        
        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }
    
    [Unit]
    public void Should_NotDeleteUser_When_IdIsNotOnDatabase()
    {
        var result = _userService.DeleteUser(NonexistentId);
        
        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_NotDeleteUser_When_UserIsActive()
    {
        var result = _userService.DeleteUser(ExistingId);
        
        Assert.Equal("Can't delete active user", result.Errors.First());
        Assert.False(result.Success);
    }
    
    [Unit]
    public void Should_DeleteUser()
    {
        var result = _userService.DeleteUser(InactiveId);
        
        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }
}
