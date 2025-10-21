using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Query.Handlers;
using ArturRios.UserManagement.Query.Queries;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Query.Tests;

public class GetUserByIdQueryTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryTests()
    {
        var userRepository = MockRepositoryFactory.CreateUserReadOnlyRepositoryInstance(_dataMock.AllUsers);
        _handler = new GetUserByIdQueryHandler(userRepository);
    }

    [UnitFact]
    public void Should_GetUserById()
    {
        var query = new GetUserByIdQuery { Id = _dataMock.ActiveIds.First() };

        var result = _handler.Handle(query);

        Assert.NotNull(result.Data);
        Assert.Equal(_dataMock.ActiveIds.First(), result.Data.Id);
        Assert.Equal($"User with Id '{_dataMock.ActiveIds.First()}' found", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_GetUserById()
    {
        var query = new GetUserByIdQuery { Id = _dataMock.NonexistentIds.First() };

        var result = _handler.Handle(query);

        Assert.Null(result.Data);
        Assert.Equal($"User with Id '{_dataMock.NonexistentIds.First()}' not found", result.Messages.First());
        Assert.True(result.Success);
    }
}
