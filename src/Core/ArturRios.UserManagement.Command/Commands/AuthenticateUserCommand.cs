namespace ArturRios.UserManagement.Command.Commands;

public class AuthenticateUserCommand : Common.Pipelines.Commands.Command
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
