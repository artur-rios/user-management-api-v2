namespace ArturRios.UserManagement.Command.Commands;

public class CreateUserCommand : Common.Pipelines.Commands.Command
{
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public int RoleId { get; set; }
}
