using System.Text;
using TechCraftsmen.Management.User.Dto;

namespace TechCraftsmen.Management.User.Test.Mock;

public static class MockExtensions
{
    public static UserDto ToDtoWithPassword(this Domain.Aggregates.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Password = Encoding.UTF8.GetString(user.Password),
            RoleId = user.RoleId,
            CreatedAt = user.CreatedAt,
            Active = user.Active
        };
    }
}