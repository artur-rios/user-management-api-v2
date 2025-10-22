using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class CreateUserCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new CreateUserCommandHandler(new CreateUserCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_CreateUser()
    {
        var mock = UserMock.New.WithRole(Roles.Regular);

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.NotNull(result.Data);
        Assert.True(result.Data.CreatedUserId > 0);
        Assert.Equal("User created successfully", result.Messages.First());
        Assert.True(result.Success);
    }

    [UnitFact]
    public void ShouldNot_CreateUser_When_UserDtoIsInvalid()
    {
        var mock = UserMock.New.WithEmail("");

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("Email should be valid", result.Errors.First());
        Assert.False(result.Success);
    }

    [UnitFact]
    public void ShouldNot_CreateUser_When_EmailIsAlreadyRegistered()
    {
        var mock = UserMock.New.WithEmail(_dataMock.ActiveEmail);

        var command = GenerateCommand(mock);

        var result = _handler.Handle(command);

        Assert.Null(result.Data);
        Assert.Equal("E-mail already registered", result.Errors.First());
        Assert.False(result.Success);
    }

    private static CreateUserCommand GenerateCommand(UserMock userMock)
    {
        var user = userMock.Generate();

        return new CreateUserCommand
        {
            Name = user.Name, Email = user.Email, Password = userMock.MockPassword, RoleId = user.RoleId
        };
    }
}
