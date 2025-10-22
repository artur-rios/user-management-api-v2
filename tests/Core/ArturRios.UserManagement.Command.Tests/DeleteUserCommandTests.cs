using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class DeleteUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new DeleteUserCommandHandler(new DeleteUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_DeleteUser()
    {
        var command = new DeleteUserCommand { UserId = _dataMock.InactiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Empty(result.Errors);
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUser_When_UserIsActive()
    {
        var command = new DeleteUserCommand { UserId = _dataMock.ActiveIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("Can't delete active user", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_DeleteUser_When_IdIsNotOnDatabase()
    {
        var command = new DeleteUserCommand { UserId = _dataMock.NonexistentIds.First() };

        var result = _handler.Handle(command);

        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }
}
