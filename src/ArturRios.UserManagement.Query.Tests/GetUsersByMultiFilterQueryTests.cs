using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Query.Handlers;
using ArturRios.UserManagement.Query.Queries;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Query.Tests;

public class GetUsersByMultiFilterQueryTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly GetUsersByMultiFilterQueryHandler _handler;

    public GetUsersByMultiFilterQueryTests()
    {
        var userRepository = MockRepositoryFactory.CreateUserReadOnlyRepositoryInstance(_dataMock.AllUsers);
        _handler = new GetUsersByMultiFilterQueryHandler(userRepository);
    }

    [UnitFact]
    public void Should_GetUsersByMultiFilter()
    {
        var query = new GetUsersByMultiFilterQuery { Ids = _dataMock.ActiveIds };

        var result = _handler.Handle(query);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(_dataMock.ActiveIds.Count, result.Data.Count);
        Assert.Contains($"Returning page {query.PageNumber} of users with page size {query.PageSize}", result.Messages);
    }

    [UnitFact]
    public void ShouldNot_GetUsersByMultiFilter()
    {
        var query = new GetUsersByMultiFilterQuery { Ids = _dataMock.NonexistentIds };

        var result = _handler.Handle(query);

        Assert.True(result.Success);
        CustomAssert.NullOrEmpty(result.Data);
        Assert.Contains("No users found for the specified filters", result.Messages);
    }
}
