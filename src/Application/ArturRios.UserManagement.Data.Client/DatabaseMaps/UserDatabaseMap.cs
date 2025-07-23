using ArturRios.UserManagement.Data.Client.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArturRios.UserManagement.Data.Client.DatabaseMaps;

public static class UserDatabaseMap
{
    public static void Configure(this EntityTypeBuilder<User> user)
    {
        user.HasKey(u => u.Id);
        
        user.Property(u => u.Name)
            .IsRequired();

        user.Property(u => u.Email)
            .IsRequired();

        user.Property(u => u.CreatedAt)
            .IsRequired();

        user.Property(u => u.Active)
            .IsRequired();

        user.Property(u => u.Role)
            .HasColumnName("RoleId")
            .HasConversion<int>();
    }
}