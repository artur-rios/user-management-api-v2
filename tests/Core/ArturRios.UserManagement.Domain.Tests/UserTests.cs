using ArturRios.Common.Test.Attributes;
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
