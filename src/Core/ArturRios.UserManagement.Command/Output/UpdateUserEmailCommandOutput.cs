using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class UpdateUserEmailCommandOutput : CommandOutput
{
    public required string UpdatedEmail { get; set; }
}
