using ArturRios.Common.Extensions;
using ArturRios.Common.Pipelines;
using ArturRios.Common.Web.Security.Interfaces;
using ArturRios.Common.Web.Security.Records;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;

namespace ArturRios.UserManagement.WebApi.Security;

public class AuthenticationProvider(Pipeline pipeline) : IAuthenticationProvider
{
    public AuthenticatedUser? GetAuthenticatedUserById(int id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var output = pipeline.ExecuteQuery<GetUserByIdQuery, UserQueryOutput>(query);

        if (!output.Success || output.Data.IsEmpty())
        {
            return null;
        }

        var user = output.Data!.First();

        return new AuthenticatedUser(user.Id, user.RoleId);

    }
}
