using TechCraftsmen.Core.Data;

namespace TechCraftsmen.Management.User.Domain.Filters;

public class UserFilter : DataFilter
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? Active { get; set; }
}
