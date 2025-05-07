using Bogus;
using Microsoft.AspNetCore.Http;
using Moq;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Dto.Mapping;
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
        var dto = UserMock.New.WithEmail("").Generate().ToDto();

        var result = _userService.CreateUser(dto);

        Assert.Equal(0, result.Data);
        Assert.Equal("Email should be valid", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_ReturnNotAllowedError_When_ForAlreadyRegisteredEmail()
    {
        var dto = UserMock.New.WithEmail(ActiveEmail).Generate().ToDtoWithPassword();
        
        var result = _userService.CreateUser(dto);
        
        Assert.Equal(0, result.Data);
        Assert.Equal("E-mail already registered", result.Messages.First());
        Assert.False(result.Success);
    }

    [Unit]
    public void Should_CreateUser_And_ReturnId_ForValidDto()
    {
        var dto = UserMock.New.Generate().ToDtoWithPassword();
        
        var result = _userService.CreateUser(dto);
        
        Assert.True(result.Data > 0);
        Assert.Equal("User created with success", result.Messages.First());
        Assert.True(result.Success);
    }
}
