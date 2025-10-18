using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class CreateUserCommandOutput : CommandOutput
{
    public int CreatedUserId { get; set; }
}
