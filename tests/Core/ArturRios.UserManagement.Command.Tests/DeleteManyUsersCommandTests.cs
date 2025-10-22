using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;
using ArturRios.UserManagement.Test.Util;

namespace ArturRios.UserManagement.Command.Tests;

public class DeleteManyUsersCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly DeleteManyUsersCommandHandler _handler;

    public DeleteManyUsersCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new DeleteManyUsersCommandHandler(new DeleteManyUsersCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_DeleteUsers()
    {
        var command = new DeleteManyUsersCommand { UserIds = _dataMock.InactiveIds };

        var result = _handler.Handle(command);

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUsers_When_UsersAreActive()
    {
        var command = new DeleteManyUsersCommand { UserIds = _dataMock.ActiveIds };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.ActiveIds)} cannot be deleted",
            result.Errors.First());

        for (var i = 0; i < _dataMock.ActiveIds.Count; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.ActiveIds)} cannot be deleted",
                    result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {_dataMock.ActiveIds[i]}: Can't delete active user",
                result.Errors.Skip(1).FirstOrDefault(e => e.ExtractIdFromDomainError() == _dataMock.ActiveIds[i]));
        }
    }

    [UnitFact]
    public void ShouldNot_DeleteUsers_When_IdsAreNotOnDatabase()
    {
        var command = new DeleteManyUsersCommand { UserIds = _dataMock.NonexistentIds };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.NonexistentIds)} not found", result.Errors.First());
    }
}
