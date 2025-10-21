using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Query.Handlers;
using ArturRios.UserManagement.Query.Queries;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Query.Tests;

public class GetUserByEmailQueryTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly GetUserByEmailQueryHandler _handler;

    public GetUserByEmailQueryTests()
    {
        var userRepository = MockRepositoryFactory.CreateUserReadOnlyRepositoryInstance(_dataMock.AllUsers);
        _handler = new GetUserByEmailQueryHandler(userRepository);
    }

    [UnitFact]
    public void Should_GetUserByEmail()
    {
        var query = new GetUserByEmailQuery { Email = _dataMock.ActiveEmail };

        var result = _handler.Handle(query);

        Assert.NotNull(result.Data);
        Assert.Equal(_dataMock.ActiveEmail, result.Data.Email);
        Assert.Equal($"User with email '{_dataMock.ActiveEmail}' found", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_GetUserByEmail()
    {
        var query = new GetUserByEmailQuery { Email = _dataMock.NonexistentEmail };

        var result = _handler.Handle(query);

        Assert.Null(result.Data);
        Assert.Equal("User with email 'nonexistent@mail.com' not found", result.Messages.First());
        Assert.True(result.Success);
    }
}
