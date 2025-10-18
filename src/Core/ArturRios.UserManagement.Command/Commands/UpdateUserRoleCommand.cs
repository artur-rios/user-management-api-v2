namespace ArturRios.UserManagement.Command.Commands;

public class UpdateUserRoleCommand : Common.Pipelines.Commands.Command
{
    public required int UserId { get; set; }
    public required int NewRoleId { get; set; }
}
