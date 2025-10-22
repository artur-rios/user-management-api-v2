using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class UpdateUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new UpdateUserCommandHandler(new UpdateUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_UpdateUser()
    {
        var mock = UserMock.New.WithId(_dataMock.ActiveIds.First());

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.NotNull(result.Data);
        Assert.Equal("User updated with success", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_UpdateUser_When_CommandIsInvalid()
    {
        var mock = UserMock.New.WithId(_dataMock.ActiveIds.First()).WithName(string.Empty);

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Name should be valid", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_NotUpdateUser_When_UserIsInactive()
    {
        var mock = UserMock.New.WithId(_dataMock.InactiveIds.First()).Inactive();

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Can't update inactive user", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void Should_NotUpdateUser_When_IdIsNotOnDatabase()
    {
        var mock = UserMock.New.WithId(_dataMock.NonexistentIds.First());

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("User not found", result.Errors.First());
        Assert.False(result.Success);
    }

    private static UpdateUserCommand GenerateCommand(UserMock userMock)
    {
        var user = userMock.Generate();

        return new UpdateUserCommand { Id = user.Id, Name = user.Name };
    }
}
