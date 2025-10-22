using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class AuthenticateUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly AuthenticateUserCommandHandler _handler;

    public AuthenticateUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreateUserReadOnlyRepositoryInstance(_dataMock.AllUsers);
        _handler = new AuthenticateUserCommandHandler(new AuthenticateUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_AuthenticateUser()
    {
        var command = new AuthenticateUserCommand
        {
            Email = _dataMock.ActiveEmail, Password = _dataMock.ActivePassword
        };

        var result = _handler.Handle(command);

        Assert.NotNull(result.Data);
        Assert.Equal(_dataMock.ActiveUserId, result.Data.Id);
        Assert.Equal(_dataMock.ActiveRoleId, result.Data.RoleId);
        Assert.Equal("User authenticated successfully", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_EmailIsInvalid()
    {
        var command = new AuthenticateUserCommand { Email = string.Empty, Password = _dataMock.ActivePassword };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Email should be valid", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_PasswordIsInvalid()
    {
        var command = new AuthenticateUserCommand { Email = _dataMock.ActiveEmail, Password = string.Empty };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Password should be valid", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_CredentialsAreInvalid()
    {
        var command = new AuthenticateUserCommand { Email = string.Empty, Password = string.Empty };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal("Email should be valid", result.Errors[0]);
        Assert.Equal("Password should be valid", result.Errors[1]);
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_EmailIsIncorrect()
    {
        var command =
            new AuthenticateUserCommand { Email = DataMock.NonexistentEmail, Password = _dataMock.ActivePassword };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Invalid credentials", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_PasswordIsIncorrect()
    {
        var command =
            new AuthenticateUserCommand { Email = _dataMock.ActiveEmail, Password = DataMock.NonexistentPassword };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Invalid credentials", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_AuthenticateUser_When_CredentialsAreIncorrect()
    {
        var command = new AuthenticateUserCommand
        {
            Email = DataMock.NonexistentEmail, Password = DataMock.NonexistentPassword
        };

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Invalid credentials", result.Errors.First());
        Assert.False(result.Success);
    }
}
