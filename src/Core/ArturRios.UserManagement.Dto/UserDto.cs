using ArturRios.UserManagement.Domain.Enums;

namespace ArturRios.UserManagement.Dto;

public class UserDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int RoleId { get; init; } = (int)Roles.Regular;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public bool Active { get; init; } = true;
}
