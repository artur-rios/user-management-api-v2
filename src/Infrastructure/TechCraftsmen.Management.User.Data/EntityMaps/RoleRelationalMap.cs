using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.EntityMaps;

internal static class RoleRelationalMap
{
    public static void Configure(this EntityTypeBuilder<Role> role)
    {
        role.HasKey(r => r.Id);
        
        role.Property(r => r.Id)
            .ValueGeneratedNever();

        role.Property(r => r.Name)
            .IsRequired();

        role.Property(r => r.Description)
            .IsRequired();
    }
}