namespace ArturRios.UserManagement.Command.Commands;

public class DeleteManyUsersCommand : Common.Pipelines.Commands.Command
{
    public required IEnumerable<int> UserIds { get; set; }
}
