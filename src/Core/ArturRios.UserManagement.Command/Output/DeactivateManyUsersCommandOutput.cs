using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class DeactivateManyUsersCommandOutput : CommandOutput
{
    public IEnumerable<int> DeactivatedIds { get; set; } = [];
    public IEnumerable<int> FailedDeactivationIds { get; set; } = [];
    public IEnumerable<int> NotFoundIds { get; set; } = [];
}
