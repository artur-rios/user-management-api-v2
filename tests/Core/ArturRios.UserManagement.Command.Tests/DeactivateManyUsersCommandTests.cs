using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;
using ArturRios.UserManagement.Test.Util;

namespace ArturRios.UserManagement.Command.Tests;

public class DeactivateManyUsersCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly DeactivateManyUsersCommandHandler _handler;

    public DeactivateManyUsersCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new DeactivateManyUsersCommandHandler(new DeactivateManyUsersCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_DeactivateUsers()
    {
        var command = new DeactivateManyUsersCommand { UserIds = _dataMock.ActiveIds };

        var result = _handler.Handle(command);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
    }

    [UnitFact]
    public void Should_NotDeactivateUsers_When_UsersAreAlreadyInactive()
    {
        var command = new DeactivateManyUsersCommand { UserIds = _dataMock.InactiveIds };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        for (var i = 0; i < _dataMock.InactiveIds.Count; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.InactiveIds)} cannot be deactivated",
                    result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {_dataMock.InactiveIds[i]}: User already inactive",
                result.Errors.Skip(1).FirstOrDefault(e => e.ExtractIdFromDomainError() == _dataMock.InactiveIds[i]));
        }
    }

    [UnitFact]
    public void Should_Not_DeactivateUsers_When_IdsAreNotOnDatabase()
    {
        var command = new DeactivateManyUsersCommand { UserIds = _dataMock.NonexistentIds };

        var result = _handler.Handle(command);


        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.NonexistentIds)} not found", result.Errors.First());
    }
}
