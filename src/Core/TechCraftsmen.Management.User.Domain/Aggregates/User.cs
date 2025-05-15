using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.Util.Condition;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Domain.Aggregates;

public class User : Entity
{
    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public byte[] Password { get; set; } = [];

    public byte[] Salt { get; set; } = [];

    public int RoleId { get; init; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Active { get; set; } = true;

    public ProcessOutput CanActivate()
    {
        return Condition.Create
            .IfNot(Active).FailsWith("User already active")
            .ToProcessOutput();
    }

    public ProcessOutput CanDeactivate()
    {
        return Condition.Create
            .If(Active).FailsWith("User already inactive")
            .ToProcessOutput();
    }

    public ProcessOutput CanDelete()
    {
        return Condition.Create
            .IfNot(Active).FailsWith("Can't delete active user")
            .ToProcessOutput();
    }

    public ProcessOutput CanRegister(int authenticatedRoleId)
    {
        return Condition.Create
            .If(authenticatedRoleId == (int)Roles.Admin || RoleId == (int)Roles.Regular)
            .FailsWith($"Only admins can register a user with {((Roles)RoleId).ToString()} role")
            .ToProcessOutput();
    }

    public ProcessOutput CanUpdate()
    {
        return Condition.Create
            .If(Active).FailsWith("Can't update inactive user")
            .ToProcessOutput();
    }
}
