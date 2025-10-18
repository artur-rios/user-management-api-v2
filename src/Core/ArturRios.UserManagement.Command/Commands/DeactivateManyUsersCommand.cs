namespace ArturRios.UserManagement.Command.Commands;

public class DeactivateManyUsersCommand : Common.Pipelines.Commands.Command
{
    public required IEnumerable<int> UserIds { get; set; }
}
