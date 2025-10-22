namespace ArturRios.UserManagement.Query.Queries;

public class GetUsersByMultiFilterQuery : Common.Pipelines.Queries.Query
{
    public IEnumerable<int>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Emails { get; set; }
    public IEnumerable<int>? RoleIds { get; set; }
}
