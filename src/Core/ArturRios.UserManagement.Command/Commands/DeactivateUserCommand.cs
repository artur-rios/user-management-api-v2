namespace ArturRios.UserManagement.Command.Commands;

public class DeactivateUserCommand : Common.Pipelines.Commands.Command
{
    public int UserId { get; set; }
}
