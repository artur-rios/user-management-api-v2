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
}
