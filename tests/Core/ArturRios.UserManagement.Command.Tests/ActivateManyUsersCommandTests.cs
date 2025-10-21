using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;
using ArturRios.UserManagement.Test.Util;

namespace ArturRios.UserManagement.Command.Tests;

public class ActivateManyUsersCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly ActivateManyUsersCommandHandler _handler;

    public ActivateManyUsersCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new ActivateManyUsersCommandHandler(new ActivateManyUsersCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_ActivateUsers()
    {
        var command = new ActivateManyUsersCommand { UserIds = _dataMock.InactiveIds };

        var result = _handler.Handle(command);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
    }

    [UnitFact]
    public void Should_Not_ActivateUsers_When_UsersAreAlreadyActive()
    {
        var command = new ActivateManyUsersCommand { UserIds = _dataMock.ActiveIds };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);

        for (var i = 0; i < _dataMock.ActiveIds.Count; i++)
        {
            if (i == 0)
            {
                Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.ActiveIds)} cannot be activated",
                    result.Errors[i]);

                continue;
            }

            Assert.Equal($"User with Id {_dataMock.ActiveIds[i]}: User already active",
                result.Errors.Skip(1).FirstOrDefault(e => e.ExtractIdFromDomainError() == _dataMock.ActiveIds[i]));
        }
    }

    [UnitFact]
    public void ShouldNot_ActivateUsers_When_IdsAreNotOnDatabase()
    {
        var command = new ActivateManyUsersCommand { UserIds = _dataMock.NonexistentIds };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal($"Users with IDs {string.Join(", ", _dataMock.NonexistentIds)} not found", result.Errors.First());
    }
}
