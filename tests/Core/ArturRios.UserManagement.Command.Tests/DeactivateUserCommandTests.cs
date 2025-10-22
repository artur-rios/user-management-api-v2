using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class DeactivateUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly DeactivateUserCommandHandler _handler;

    public DeactivateUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new DeactivateUserCommandHandler(new DeactivateUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_DeactivateUser()
    {
        var command = new DeactivateUserCommand { UserId = _dataMock.ActiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeactivateUser_When_IdIsNotOnDatabase()
    {
        var command = new DeactivateUserCommand { UserId = _dataMock.NonexistentIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeactivateUser_When_UserIsAlreadyInactive()
    {
        var command = new DeactivateUserCommand { UserId = _dataMock.InactiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("User already inactive", result.Errors.First());
        Assert.False(result.Success);
    }
}
