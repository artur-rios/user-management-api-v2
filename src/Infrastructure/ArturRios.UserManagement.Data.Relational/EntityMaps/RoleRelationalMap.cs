using ArturRios.UserManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArturRios.UserManagement.Data.Relational.EntityMaps;

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