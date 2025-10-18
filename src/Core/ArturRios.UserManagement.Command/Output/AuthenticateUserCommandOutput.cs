using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class AuthenticateUserCommandOutput : CommandOutput
{
    public int Id { get; set; }
    public int RoleId { get; set; }
}
