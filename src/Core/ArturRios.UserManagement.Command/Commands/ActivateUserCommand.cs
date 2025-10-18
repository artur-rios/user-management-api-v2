namespace ArturRios.UserManagement.Command.Commands;

public class ActivateUserCommand : Common.Pipelines.Commands.Command
{
    public int UserId { get; set; }
}
