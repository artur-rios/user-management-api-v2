using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.EntityMaps;

internal static class RoleRelationalMap
{
    public static void Configure(this EntityTypeBuilder<Role> role)
    {
        role.HasKey(r => r.Id);
    }
}