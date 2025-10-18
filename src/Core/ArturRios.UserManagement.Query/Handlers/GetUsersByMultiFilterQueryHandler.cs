using ArturRios.Common.Extensions;
using ArturRios.Common.Output;
using ArturRios.Common.Pipelines.Queries;
using ArturRios.UserManagement.Domain.Repositories;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;
using Microsoft.EntityFrameworkCore;

namespace ArturRios.UserManagement.Query.Handlers;

public class GetUsersByMultiFilterQueryHandler(IUserReadOnlyRepository userRepository)
    : IQueryHandler<GetUsersByMultiFilterQuery, UserQueryOutput>
{
    public PaginatedOutput<UserQueryOutput> Handle(GetUsersByMultiFilterQuery query)
    {
        var dbQuery = userRepository.GetAll().AsNoTracking();

        if (query.Ids.IsNotEmpty())
        {
            dbQuery = dbQuery.Where(e => query.Ids!.Contains(e.Id));
        }

        if (query.Names.IsNotEmpty())
        {
            dbQuery = dbQuery.Where(e => query.Names!.Contains(e.Name));
        }

        if (query.Emails.IsNotEmpty())
        {
            dbQuery = dbQuery.Where(e => query.Emails!.Contains(e.Email));
        }

        if (query.RoleIds.IsNotEmpty())
        {
            dbQuery = dbQuery.Where(e => query.RoleIds!.Contains(e.RoleId));
        }

        return dbQuery.Select(e => new UserQueryOutput
        {
            Id = e.Id,
            Name = e.Name,
            Email = e.Email,
            RoleId = e.RoleId,
            CreatedAt = e.CreatedAt,
            Active = e.Active
        }).Paginate(query.PageNumber, query.PageSize);
    }
}
