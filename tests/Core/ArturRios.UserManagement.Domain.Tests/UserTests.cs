using ArturRios.Common.Test.Attributes;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.Domain.Tests;

public class UserTests
{
    [UnitFact]
    public void Should_AllowActivation()
    {
        var user = UserMock.New.Inactive().Generate();

        var canActivate = user.Activate();

        Assert.True(canActivate.Success);
        Assert.Empty(canActivate.Errors);
    }

    [UnitFact]
    public void ShouldNot_AllowActivation_WhenUserIsAlreadyActive()
    {
        var user = UserMock.New.Active().Generate();

        var canActivate = user.Activate();

        Assert.False(canActivate.Success);
        Assert.NotEmpty(canActivate.Errors);
        Assert.Equal("User already active", canActivate.Errors.FirstOrDefault());
    }

    [UnitFact]
    public void Should_AllowDeactivation()
    {
        var user = UserMock.New.Active().Generate();

        var canDeactivate = user.Deactivate();

        Assert.True(canDeactivate.Success);
        Assert.Empty(canDeactivate.Errors);
    }

    [UnitFact]
    public void ShouldNot_AllowDeactivation_When_UserIsAlreadyInactive()
    {
        var user = UserMock.New.Inactive().Generate();

        var canDeactivate = user.Deactivate();

        Assert.False(canDeactivate.Success);
        Assert.NotEmpty(canDeactivate.Errors);
        Assert.Equal("User already inactive", canDeactivate.Errors.FirstOrDefault());
    }

    [UnitFact]
    public void Should_UpdateRole()
    {
        var user = UserMock.New.Active().WithRole(Roles.Regular).Generate();

        var result = user.SetRole((int)Roles.Test);

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
        Assert.Equal((int)Roles.Test, user.RoleId);
    }

    [UnitFact]
    public void ShouldNot_UpdateRole_When_RoleIsInvalid()
    {
        var user = UserMock.New.Active().WithRole(Roles.Regular).Generate();

        var result = user.SetRole(999);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("Role should be valid", result.Errors.FirstOrDefault());
        Assert.Equal((int)Roles.Regular, user.RoleId);
    }

    [UnitFact]
    public void ShouldNot_UpdateRole_When_RoleIsSame()
    {
        var user = UserMock.New.Active().WithRole(Roles.Regular).Generate();

        var result = user.SetRole((int)Roles.Regular);

        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
        Assert.Equal("New role must be different from current role", result.Errors.FirstOrDefault());
        Assert.Equal((int)Roles.Regular, user.RoleId);
    }

    [UnitFact]
    public void Should_AllowUpdate()
    {
        var user = UserMock.New.Active().Generate();
        var updatedUser = UserMock.New.Active().Generate();

        var canUpdate = user.Update(updatedUser);

        Assert.True(canUpdate.Success);
        Assert.Empty(canUpdate.Errors);
    }

    [UnitFact]
    public void ShouldNot_AllowUpdate_When_UserIsInactive()
    {
        var user = UserMock.New.Inactive().Generate();
        var updatedUser = UserMock.New.Active().Generate();

        var canUpdate = user.Update(updatedUser);

        Assert.False(canUpdate.Success);
        Assert.NotEmpty(canUpdate.Errors);
        Assert.Equal("Can't update inactive user", canUpdate.Errors.FirstOrDefault());
    }

    [UnitFact]
    public void Should_AllowDeletion()
    {
        var user = UserMock.New.Inactive().Generate();

        var canDelete = user.CanDelete();

        Assert.True(canDelete.Success);
        Assert.Empty(canDelete.Errors);
    }

    [UnitFact]
    public void ShouldNot_AllowDeletion_When_UserIsActive()
    {
        var user = UserMock.New.Active().Generate();

        var canDelete = user.CanDelete();

        Assert.False(canDelete.Success);
        Assert.NotEmpty(canDelete.Errors);
        Assert.Equal("Can't delete active user", canDelete.Errors.FirstOrDefault());
    }
}
