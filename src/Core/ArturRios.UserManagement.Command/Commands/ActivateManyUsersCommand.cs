namespace ArturRios.UserManagement.Command.Commands;

public class ActivateManyUsersCommand : Common.Pipelines.Commands.Command
{
    public required IEnumerable<int> UserIds { get; set; }
}
