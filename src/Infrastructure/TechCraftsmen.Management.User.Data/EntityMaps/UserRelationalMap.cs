using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechCraftsmen.Management.User.Domain.Aggregates;

namespace TechCraftsmen.Management.User.Data.EntityMaps;

internal static class UserRelationalMap
{
    public static void Configure(this EntityTypeBuilder<Domain.Aggregates.User> user)
    {
        user.HasKey(u => u.Id);
        
        user.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
    }
}
