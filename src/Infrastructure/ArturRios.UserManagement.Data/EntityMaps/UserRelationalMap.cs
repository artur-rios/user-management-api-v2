using ArturRios.UserManagement.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArturRios.UserManagement.Data.EntityMaps;

internal static class UserRelationalMap
{
    public static void Configure(this EntityTypeBuilder<User> user)
    {
        user.HasKey(u => u.Id);
        
        user.Property(u => u.Name)
            .IsRequired();

        user.Property(u => u.Email)
            .IsRequired();

        user.Property(u => u.Password)
            .IsRequired();

        user.Property(u => u.Salt)
            .IsRequired();

        user.Property(u => u.RoleId)
            .IsRequired();

        user.Property(u => u.CreatedAt)
            .IsRequired();

        user.Property(u => u.Active)
            .IsRequired();
        
        user.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
    }
}
