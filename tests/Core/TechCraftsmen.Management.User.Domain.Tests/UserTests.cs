using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.Domain.Tests;

public class UserTests
{
    [Unit]
    public void Should_NotAllowCreationOfAdmins_IfAuthenticatedUserNotAnAdmin()
    {
        var user = UserMock.New.WithRole(Roles.Admin).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Regular);

        Assert.False(canRegister.Success);
        Assert.NotEmpty(canRegister.Errors);
        Assert.Equal("Only admins can register a user with Admin role", canRegister.Errors.FirstOrDefault());
    }

    [Unit]
    public void Should_NotAllowDeletion_ForActiveUser()
    {
        var user = UserMock.New.Active().Generate();
        
        var canDelete = user.CanDelete();
        
        Assert.False(canDelete.Success);
        Assert.NotEmpty(canDelete.Errors);
        Assert.Equal("Can't delete active user", canDelete.Errors.FirstOrDefault());
    }

    [Unit]
    public void Should_AllowDeletion_ForInactiveUser()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canDelete = user.CanDelete();
        
        Assert.True(canDelete.Success);
        Assert.Empty(canDelete.Errors);
    }
    
    [Unit]
    public void Should_NotAllowActivation_ForActiveUser()
    {
        var user = UserMock.New.Active().Generate();
        
        var canActivate = user.CanActivate();
        
        Assert.False(canActivate.Success);
        Assert.NotEmpty(canActivate.Errors);
        Assert.Equal("User already active", canActivate.Errors.FirstOrDefault());
    }

    [Unit]
    public void Should_AllowActivation_ForInactiveUser()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canActivate = user.CanActivate();
        
        Assert.True(canActivate.Success);
        Assert.Empty(canActivate.Errors);
    }
    
    [Unit]
    public void Should_NotAllowDeactivation_ForInactiveUser()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canDeactivate = user.CanDeactivate();
        
        Assert.False(canDeactivate.Success);
        Assert.NotEmpty(canDeactivate.Errors);
        Assert.Equal("User already inactive", canDeactivate.Errors.FirstOrDefault());
    }
    
    [Unit]
    public void Should_AllowDeactivation_ForActiveUser()
    {
        var user = UserMock.New.Active().Generate();
        
        var canDeactivate = user.CanDeactivate();
        
        Assert.True(canDeactivate.Success);
        Assert.Empty(canDeactivate.Errors);
    }

    [Unit]
    public void Should_NotAllowUpdate_InactiveUser()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canUpdate = user.CanUpdate();
        
        Assert.False(canUpdate.Success);
        Assert.NotEmpty(canUpdate.Errors);
        Assert.Equal("Can't update inactive user", canUpdate.Errors.FirstOrDefault());
    }
    
    [Unit]
    public void Should_AllowUpdate_ForActiveUser()
    {
        var user = UserMock.New.Active().Generate();
        
        var canUpdate = user.CanUpdate();
        
        Assert.True(canUpdate.Success);
        Assert.Empty(canUpdate.Errors);
    }
}
