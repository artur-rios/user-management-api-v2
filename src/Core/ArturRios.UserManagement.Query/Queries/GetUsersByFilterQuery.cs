namespace ArturRios.UserManagement.Query.Queries;

public class GetUsersByFilterQuery : Common.Pipelines.Queries.Query
{
    public string? Name { get; set; }
    public int? RoleId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? Active { get; set; }
}
