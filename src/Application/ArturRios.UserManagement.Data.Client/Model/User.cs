using ArturRios.UserManagement.Domain.Enums;

namespace ArturRios.UserManagement.Data.Client.Model;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public Roles Role { get; set; }

    public DateTime CreatedAt { get; init; }

    public bool Active { get; init; }
}
