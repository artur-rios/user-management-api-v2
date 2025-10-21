using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class ActivateUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly ActivateUserCommandHandler _handler;

    public ActivateUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new ActivateUserCommandHandler(new ActivateUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_ActivateUser()
    {
        var command = new ActivateUserCommand { UserId = _dataMock.InactiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_ActivateUser_When_IdIsNotOnDatabase()
    {
        var command = new ActivateUserCommand { UserId = _dataMock.NonexistentIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_ActivateUser_When_UserIsAlreadyActive()
    {
        var command = new ActivateUserCommand { UserId = _dataMock.ActiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("User already active", result.Errors.First());
        Assert.False(result.Success);
    }
}
