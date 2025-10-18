using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class ActivateManyUsersCommandOutput : CommandOutput
{
    public IEnumerable<int> ActivatedIds { get; set; } = [];
    public IEnumerable<int> FailedActivationIds { get; set; } = [];
    public IEnumerable<int> NotFoundIds { get; set; } = [];
}
