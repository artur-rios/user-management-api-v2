using ArturRios.Common.Pipelines.Queries;

namespace ArturRios.UserManagement.Query.Output;

public class UserQueryOutput : QueryOutput
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool Active { get; set; }
}
