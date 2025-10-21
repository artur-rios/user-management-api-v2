using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Query.Handlers;
using ArturRios.UserManagement.Query.Queries;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Query.Tests;

public class GetUsersByFilterQueryTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly GetUsersByFilterQueryHandler _handler;

    public GetUsersByFilterQueryTests()
    {
        var userRepository = MockRepositoryFactory.CreateUserReadOnlyRepositoryInstance(_dataMock.AllUsers);
        _handler = new GetUsersByFilterQueryHandler(userRepository);
    }

    [UnitFact]
    public void Should_GetUsersByFilter()
    {
        var query = new GetUsersByFilterQuery { Active = true };

        var result = _handler.Handle(query);

        Assert.NotNull(result.Data);
        Assert.NotEmpty(result.Data);
        Assert.Equal(_dataMock.ActiveUsers.Count, result.Data.Count);
        Assert.Equal($"Returning page {query.PageNumber} of users with page size {query.PageSize}", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_GetUsers_When_FilterDoesNotMatchAnyUser()
    {
        var query = new GetUsersByFilterQuery { Name = "99999" };

        var result = _handler.Handle(query);

        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal("No users found for the specified filters", result.Messages.First());
        Assert.True(result.Success);
    }
}
