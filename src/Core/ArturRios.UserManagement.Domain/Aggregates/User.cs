using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArturRios.Common.Data;
using ArturRios.Common.Output;
using ArturRios.Common.Util.Condition;

namespace ArturRios.UserManagement.Domain.Aggregates;

public class User : Entity
{
    [Column(Order = 1)]
    [MaxLength(300)]
    public string Name { get; private set; } = string.Empty;

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
        var condition = Condition.Create.IfNot(Active).FailsWith("User already active");
        
        if (condition.IsSatisfied)
        {
            Active = true;
        }
        
        return condition.ToProcessOutput();
    }

    public ProcessOutput CanDelete()
    {
        return Condition.Create
            .IfNot(Active).FailsWith("Can't delete active user")
            .ToProcessOutput();
    }
    
    public ProcessOutput Deactivate()
    {
        var condition = Condition.Create.If(Active).FailsWith("User already inactive");
        
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

    public ProcessOutput Update(User updatedUser)
    {
        var condition = Condition.Create.If(Active).FailsWith("Can't update inactive user");
        
        if (condition.IsSatisfied)
        {
            Name = updatedUser.Name;
            Email = updatedUser.Email;
        }
        
        return condition.ToProcessOutput();
    }
}
