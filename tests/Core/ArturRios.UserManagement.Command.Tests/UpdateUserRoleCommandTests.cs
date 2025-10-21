using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Command.CommandHandlers;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Validators;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Command.Tests;

public class UpdateUserRoleCommandTests
{
    private readonly DataMock _dataMock = DataMock.New.Build();
    private readonly UpdateUserRoleCommandHandler _handler;

    public UpdateUserRoleCommandTests()
    {
        var userRepository = MockRepositoryFactory.CreatUserRepositoryInstance(_dataMock.AllUsers);
        _handler = new UpdateUserRoleCommandHandler(new UpdateUserRoleCommandValidator(), userRepository);
    }

    [UnitFact]
    public void Should_UpdateUserRole()
    {
        var command = new UpdateUserRoleCommand { UserId = _dataMock.ActiveIds.First(), NewRoleId = (int)Roles.Test };

        var result = _handler.Handle(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(command.NewRoleId, result.Data.UpdatedUserRoleId);
        Assert.Equal($"User role updated to {command.NewRoleId}", result.Messages.First());
    }

    [UnitFact]
    public void ShouldNot_UpdateUserRole_When_IdIsNotOnDatabase()
    {
        var command =
            new UpdateUserRoleCommand { UserId = _dataMock.NonexistentIds.First(), NewRoleId = (int)Roles.Test };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("User not found", result.Errors.First());
    }

    [UnitFact]
    public void ShouldNot_UpdateUserRole_When_RoleIsInvalid()
    {
        var command = new UpdateUserRoleCommand { UserId = _dataMock.ActiveIds.First(), NewRoleId = 999 };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("Role should be valid", result.Errors.First());
    }

    [UnitFact]
    public void ShouldNot_UpdateUserRole_When_RoleIsSame()
    {
        var command = new UpdateUserRoleCommand { UserId = _dataMock.ActiveIds.First(), NewRoleId = (int)Roles.Regular };

        var result = _handler.Handle(command);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("New role must be different from current role", result.Errors.FirstOrDefault());
    }
}
