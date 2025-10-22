using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class UpdateUserEmailCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly UpdateUserEmailCommandHandler _handler;

    public UpdateUserEmailCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new UpdateUserEmailCommandHandler(new UpdateUserEmailCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_UpdateUserEmail()
    {
        var command = new UpdateUserEmailCommand { UserId = _dataMock.ActiveUserId, Email = "updated@mail.com" };

        var result = _handler.Handle(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(command.Email, result.Data.UpdatedEmail);
        Assert.Equal($"User e-mail updated to {command.Email}", result.Messages.First());
    }

    [UnitFact]
    public void Should_Not_UpdateUserEmail_WhenEmailIsInvalid()
    {
        var command = new UpdateUserEmailCommand { UserId = _dataMock.ActiveUserId, Email = string.Empty };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("Email should be valid", result.Errors.First());
    }

    [UnitFact]
    public void Should_Not_UpdateUserEmail_WhenIdIsNotOnDatabase()
    {
        var command =
            new UpdateUserEmailCommand { UserId = _dataMock.NonexistentIds.First(), Email = "updated@mail.com" };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("User not found", result.Errors.First());
    }

    [UnitFact]
    public void Should_Not_UpdateUserEmail_WhenEmailIsAlreadyInUse()
    {
        var command = new UpdateUserEmailCommand
        {
            UserId = _dataMock.ActiveUserId, Email = _dataMock.ActiveUsers[1].Email
        };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("Email is already in use", result.Errors.FirstOrDefault());
    }

    [UnitFact]
    public void Should_Not_UpdateUserEmail_WhenEmailIsSame()
    {
        var command = new UpdateUserEmailCommand { UserId = _dataMock.ActiveUserId, Email = _dataMock.ActiveEmail };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("New email must be different from current email", result.Errors.FirstOrDefault());
    }
}
