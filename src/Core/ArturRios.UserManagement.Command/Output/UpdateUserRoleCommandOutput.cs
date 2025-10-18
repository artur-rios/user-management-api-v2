using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class UpdateUserRoleCommandOutput : CommandOutput
{
    public required int UpdatedUserRoleId { get; set; }
}
