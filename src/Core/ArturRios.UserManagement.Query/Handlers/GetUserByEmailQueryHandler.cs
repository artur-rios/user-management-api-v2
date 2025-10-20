using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Queries;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;

namespace ArturRios.UserManagement.Query.Handlers;

public class GetUserByEmailQueryHandler(IUserReadOnlyRepository userRepository)
    : ISingleQueryHandler<GetUserByEmailQuery, UserQueryOutput>
{
    public DataOutput<UserQueryOutput?> Handle(GetUserByEmailQuery query)
    {
        var user = userRepository.GetAll().FirstOrDefault(u => u.Email == query.Email);

        if (user is null)
        {
            return DataOutput<UserQueryOutput?>.New
                .WithMessage($"User with email '{query.Email}' not found");
        }

        var output = new UserQueryOutput
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            RoleId = user.RoleId,
            CreatedAt = user.CreatedAt,
            Active = user.Active
        };

        return DataOutput<UserQueryOutput?>.New
            .WithData(output)
            .WithMessage($"User with email '{query.Email}' found");
    }
}
