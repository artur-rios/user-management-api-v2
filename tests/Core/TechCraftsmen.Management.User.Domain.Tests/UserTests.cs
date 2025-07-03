using TechCraftsmen.Core.Test.Attributes;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Test.Mock;

namespace TechCraftsmen.Management.User.Domain.Tests;

public class UserTests
{
    [UnitFact]
    public void Should_AllowCreationOfAdmin()
    {
        var user = UserMock.New.WithRole(Roles.Admin).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Admin);

        Assert.True(canRegister.Success);
        Assert.Empty(canRegister.Errors);
    }
    
    [UnitFact]
    public void Should_AllowCreationOfRegularUser()
    {
        var user = UserMock.New.WithRole(Roles.Regular).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Regular);

        Assert.True(canRegister.Success);
        Assert.Empty(canRegister.Errors);
    }
    
    [UnitFact]
    public void Should_AllowCreationOfTestUser()
    {
        var user = UserMock.New.WithRole(Roles.Test).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Admin);

        Assert.True(canRegister.Success);
        Assert.Empty(canRegister.Errors);
    }
    
    [UnitFact]
    public void ShouldNot_AllowCreationOfAdmin_When_AuthenticatedUserIsNotAnAdmin()
    {
        var user = UserMock.New.WithRole(Roles.Admin).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Regular);

        Assert.False(canRegister.Success);
        Assert.NotEmpty(canRegister.Errors);
        Assert.Equal("Only admins can register a user with Admin role", canRegister.Errors.FirstOrDefault());
    }
    
    [UnitFact]
    public void ShouldNot_AllowCreationOfTestUser_When_AuthenticatedUserIsNotAnAdmin()
    {
        var user = UserMock.New.WithRole(Roles.Test).Generate();
        
        var canRegister = user.CanRegister((int)Roles.Regular);

        Assert.False(canRegister.Success);
        Assert.NotEmpty(canRegister.Errors);
        Assert.Equal("Only admins can register a user with Test role", canRegister.Errors.FirstOrDefault());
    }
    
    [UnitFact]
    public void Should_AllowActivation()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canActivate = user.CanActivate();
        
        Assert.True(canActivate.Success);
        Assert.Empty(canActivate.Errors);
    }
    
    [UnitFact]
    public void Should_NotAllowActivation_WhenUserIsAlreadyActive()
    {
        var user = UserMock.New.Active().Generate();
        
        var canActivate = user.CanActivate();
        
        Assert.False(canActivate.Success);
        Assert.NotEmpty(canActivate.Errors);
        Assert.Equal("User already active", canActivate.Errors.FirstOrDefault());
    }
    
    [UnitFact]
    public void Should_AllowDeactivation()
    {
        var user = UserMock.New.Active().Generate();
        
        var canDeactivate = user.CanDeactivate();
        
        Assert.True(canDeactivate.Success);
        Assert.Empty(canDeactivate.Errors);
    }
    
    [UnitFact]
    public void Should_NotAllowDeactivation_When_UserIsAlreadyInactive()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canDeactivate = user.CanDeactivate();
        
        Assert.False(canDeactivate.Success);
        Assert.NotEmpty(canDeactivate.Errors);
        Assert.Equal("User already inactive", canDeactivate.Errors.FirstOrDefault());
    }
    
    [UnitFact]
    public void Should_AllowUpdate()
    {
        var user = UserMock.New.Active().Generate();
        
        var canUpdate = user.CanUpdate();
        
        Assert.True(canUpdate.Success);
        Assert.Empty(canUpdate.Errors);
    }

    [UnitFact]
    public void Should_NotAllowUpdate_When_UserIsInactive()
    {
        var user = UserMock.New.Inactive().Generate();
        
        var canUpdate = user.CanUpdate();
        
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
