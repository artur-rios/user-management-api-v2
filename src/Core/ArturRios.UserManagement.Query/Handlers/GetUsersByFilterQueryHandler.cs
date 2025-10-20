using ArturRios.Common.Extensions;
using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Queries;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Query.Handlers;

public class GetUsersByFilterQueryHandler(IUserReadOnlyRepository userRepository)
    : IQueryHandler<GetUsersByFilterQuery, UserQueryOutput>
{
    public PaginatedOutput<UserQueryOutput> Handle(GetUsersByFilterQuery query)
    {
        var dbQuery = userRepository.GetAll().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            dbQuery = dbQuery.Where(e => e.Name == query.Name);
        }

        if (query.RoleId.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.RoleId == query.RoleId.Value);
        }

        if (query.CreatedAt.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.CreatedAt == query.CreatedAt.Value);
        }

        if (query.Active.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.Active == query.Active.Value);
        }

        var output = dbQuery.Select(e => new UserQueryOutput
        {
            Id = e.Id,
            Name = e.Name,
            Email = e.Email,
            RoleId = e.RoleId,
            CreatedAt = e.CreatedAt,
            Active = e.Active
        }).Paginate(query.PageNumber, query.PageSize);

        if (output.Data is not null && output.Data.Count > 0)
        {
            output.AddMessage($"Returning page {query.PageNumber} of users with page size {query.PageSize}");
        }
        else
        {
            output.AddMessage("No users found for the specified filters");
        }

        return output;
    }
}
