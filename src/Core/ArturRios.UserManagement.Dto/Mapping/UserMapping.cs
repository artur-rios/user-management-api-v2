namespace ArturRios.UserManagement.Dto.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this Domain.Aggregates.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            RoleId = user.RoleId,
            CreatedAt = user.CreatedAt,
            Active = user.Active
        };
    }

    public static Domain.Aggregates.User ToEntity(this UserDto dto)
    {
        return new Domain.Aggregates.User
        {
            Id = dto.Id,
            Email = dto.Email,
            Name = dto.Name,
            RoleId = dto.RoleId,
            CreatedAt = dto.CreatedAt,
            Active = dto.Active
        };
    }
}
