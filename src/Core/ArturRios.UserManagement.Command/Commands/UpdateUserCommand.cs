namespace ArturRios.UserManagement.Command.Commands;

public class UpdateUserCommand : Common.Pipelines.Commands.Command
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
