using ArturRios.Common.Pipelines.Queries;
using ArturRios.UserManagement.Query.Handlers;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace ArturRios.UserManagement.IoC.DependencyInjection;

public static class QueryProvider
{
    public static void AddQueries(this IServiceCollection services)
    {
        services.AddScoped<ISingleQueryHandler<GetUserByEmailQuery, UserQueryOutput>, GetUserByEmailQueryHandler>();
        services.AddScoped<ISingleQueryHandler<GetUserByIdQuery, UserQueryOutput>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersByFilterQuery, UserQueryOutput>, GetUsersByFilterQueryHandler>();
        services
            .AddScoped<IQueryHandler<GetUsersByMultiFilterQuery, UserQueryOutput>, GetUsersByMultiFilterQueryHandler>();
    }
}
