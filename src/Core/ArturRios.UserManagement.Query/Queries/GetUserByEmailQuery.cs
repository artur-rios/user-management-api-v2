namespace ArturRios.UserManagement.Query.Queries;

public class GetUserByEmailQuery : Common.Pipelines.Queries.Query
{
    public required string Email { get; set; }
}
