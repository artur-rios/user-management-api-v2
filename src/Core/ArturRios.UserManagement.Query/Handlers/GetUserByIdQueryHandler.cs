using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Queries;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;

namespace ArturRios.UserManagement.Query.Handlers;

public class GetUserByIdQueryHandler(IUserReadOnlyRepository userRepository)
    : ISingleQueryHandler<GetUserByIdQuery, UserQueryOutput>
{
    public DataOutput<UserQueryOutput?> Handle(GetUserByIdQuery query)
    {
        var user = userRepository.GetById(query.Id);

        if (user is null)
        {
            return DataOutput<UserQueryOutput?>.New
                .WithMessage($"User with Id '{query.Id}' not found");
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
            .WithMessage($"User with Id '{query.Id}' found");
    }
}
