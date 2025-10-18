using ArturRios.Common.Web.Security.Records;
using ArturRios.UserManagement.Domain.Enums;
using Bogus;

namespace ArturRios.UserManagement.Test.Mock;

public class AuthenticatedUserMock
{
    private readonly AuthenticatedUser _mockAuthenticatedUser;
    private readonly Faker _faker = new();

    public AuthenticatedUserMock()
    {
        _mockAuthenticatedUser = new AuthenticatedUser(_faker.Random.Int(1, 1000), _faker.PickRandom((int)Roles.Admin, (int)Roles.Regular));
    }

    public AuthenticatedUserMock(Roles role)
    {
        _mockAuthenticatedUser = new AuthenticatedUser(_faker.Random.Int(1, 1000), (int)role);
    }

    public AuthenticatedUser Generate()
    {
        return _mockAuthenticatedUser;
    }
}
