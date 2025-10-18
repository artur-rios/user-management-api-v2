using ArturRios.Common.Pipelines.Commands;

namespace ArturRios.UserManagement.Command.Output;

public class DeleteManyUsersCommandOutput : CommandOutput
{
    public IEnumerable<int> DeletedIds { get; set; } = [];
    public IEnumerable<int> FailedDeletionIds { get; set; } = [];
    public IEnumerable<int> NotFoundIds { get; set; } = [];
}
