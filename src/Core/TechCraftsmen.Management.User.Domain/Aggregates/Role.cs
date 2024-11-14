using TechCraftsmen.Core.Data;

namespace TechCraftsmen.Management.User.Domain.Aggregates;

public class Role : Entity
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;  
}
