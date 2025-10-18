using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class UpdateUserCommandOutput : CommandOutput
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Active { get; set; }
}
