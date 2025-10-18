namespace ArturRios.UserManagement.Command.Commands;

public class UpdateUserEmailCommand : Common.Pipelines.Commands.Command
{
    public required int Id { get; set; }
    public required string Email { get; set; }
}
