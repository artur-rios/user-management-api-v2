using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArturRios.Common.Data;
using ArturRios.Common.Output;
using ArturRios.Common.Util.Condition;
using ArturRios.UserManagement.Domain.Enums;

namespace ArturRios.UserManagement.Domain.Aggregates;

public class User : Entity
{
    [Column(Order = 1)]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Column(Order = 2)]
    [MaxLength(300)]
    public string Email { get; private set; } = string.Empty;

    [Column(Order = 3)]
    public byte[] Password { get;  private set; } = [];

    [Column(Order = 4)]
    public byte[] Salt { get;  private set; } = [];

    [Column(Order = 5)]
    public int RoleId { get;  private set; }

    [Column(Order = 6)]
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    [Column(Order = 7)]
    public bool Active { get; private set; } = true;

    public User()
    {
    }

    public User(string email, string name, int roleId)
    {
        Email = email;
        Name = name;
        RoleId = roleId;
    }

    public User(string email, string name, int roleId, DateTime createdAt, bool active = true)
    {
        Email = email;
        Name = name;
        RoleId = roleId;
        CreatedAt = createdAt;
        Active = active;
    }

    public User(int id, string email, string name, int roleId, DateTime createdAt, bool active)
    {
        Id = id;
        Email = email;
        Name = name;
        RoleId = roleId;
        CreatedAt = createdAt;
        Active = active;
    }

    public ProcessOutput Activate()
    {
        var condition = Condition.Create.False(Active).FailsWith("User already active");

        if (condition.IsSatisfied)
        {
            Active = true;
        }

        return condition.ToProcessOutput();
    }

    public ProcessOutput CanDelete()
    {
        return Condition.Create
            .False(Active).FailsWith("Can't delete active user")
            .ToProcessOutput();
    }

    public ProcessOutput Deactivate()
    {
        var condition = Condition.Create.True(Active).FailsWith("User already inactive");

        if (condition.IsSatisfied)
        {
            Active = false;
        }

        return condition.ToProcessOutput();
    }

    public void SetPassword(byte[] password, byte[] salt)
    {
        Password = password;
        Salt = salt;
    }

    public ProcessOutput Update(string name)
    {
        var condition = Condition.Create
            .True(Active).FailsWith("Can't update inactive user")
            .False(string.IsNullOrWhiteSpace(name)).FailsWith("Name cannot be empty");

        if (condition.IsSatisfied)
        {
            Name = name;
        }

        return condition.ToProcessOutput();
    }

    public ProcessOutput UpdateEmail(string email)
    {
        var condition = Condition.Create
            .False(string.IsNullOrWhiteSpace(email)).FailsWith("Email cannot be empty")
            .False(email == Email).FailsWith("New email must be different from current email")
            .True(Active).FailsWith("Can't change email of inactive user");

        if (condition.IsSatisfied)
        {
            Email = email;
        }

        return condition.ToProcessOutput();
    }

    public ProcessOutput UpdateRole(int roleId)
    {
        var validRole = Enum.IsDefined(typeof(Roles), roleId);
        var condition = Condition.Create
            .True(validRole).FailsWith("Role should be valid")
            .True(Active).FailsWith("Can't change role of inactive user")
            .True(roleId != RoleId).FailsWith("New role must be different from current role");

        if (condition.IsSatisfied)
        {
            RoleId = roleId;
        }

        return condition.ToProcessOutput();
    }
}
