namespace ArturRios.UserManagement.Command.Commands;

public class DeleteUserCommand : Common.Pipelines.Commands.Command
{
    public required int UserId { get; set; }
}
