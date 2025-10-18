using ArturRios.Common.Output;
using ArturRios.Common.Pipelines;
using ArturRios.Common.Web.Api.Base;
using ArturRios.Common.Web.Api.Output;
using ArturRios.Common.Web.Security.Attributes;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Query.Output;
using ArturRios.UserManagement.Query.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController(Pipeline pipeline) : BaseController
{
    [HttpPost]
    [Route("Create/Admin")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<CreateUserCommandOutput>> CreateAdminUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Admin;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPost]
    [Route("Create/Regular")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<CreateUserCommandOutput>> CreateRegularUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Regular;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPost]
    [Route("Create/Test")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<CreateUserCommandOutput>> CreateTestUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Test;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpGet]
    [Route("{id:int}")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<PaginatedOutput<UserQueryOutput>>> GetUserById([FromRoute] int id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var output = pipeline.ExecuteQuery<GetUserByIdQuery, UserQueryOutput>(query);

        return Resolve(output);
    }

    [HttpGet]
    [Route("Filter")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<PaginatedOutput<UserQueryOutput>>> GetUsersByFilter(
        [FromQuery] GetUsersByFilterQuery query)
    {
        var output = pipeline.ExecuteQuery<GetUsersByFilterQuery, UserQueryOutput>(query);

        return Resolve(output);
    }

    [HttpGet]
    [Route("Filter/Multi")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<PaginatedOutput<UserQueryOutput>>> GetUsersByMultiFilter(
        [FromQuery] GetUsersByMultiFilterQuery query)
    {
        var output = pipeline.ExecuteQuery<GetUsersByMultiFilterQuery, UserQueryOutput>(query);

        return Resolve(output);
    }

    [HttpPut]
    [Route("Update")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UpdateUserCommandOutput>> UpdateUser([FromBody] UpdateUserCommand command)
    {
        var output = pipeline.ExecuteCommand<UpdateUserCommand, UpdateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPatch]
    [Route("Update/Role")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UpdateUserRoleCommandOutput>> UpdateUserRole(
        [FromBody] UpdateUserRoleCommand command)
    {
        var output = pipeline.ExecuteCommand<UpdateUserRoleCommand, UpdateUserRoleCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPatch]
    [Route("{id:int}/Activate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<ActivateUserCommandOutput>> ActivateUser([FromRoute] int id)
    {
        var command = new ActivateUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<ActivateUserCommand, ActivateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPatch]
    [Route("ActivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<ActivateManyUsersCommandOutput>> ActivateManyUsers([FromBody] int[] ids)
    {
        var command = new ActivateManyUsersCommand { UserIds = ids };
        var output = pipeline.ExecuteCommand<ActivateManyUsersCommand, ActivateManyUsersCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPatch]
    [Route("{id:int}/Deactivate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<DeactivateUserCommandOutput>> DeactivateUser([FromRoute] int id)
    {
        var command = new DeactivateUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<DeactivateUserCommand, DeactivateUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpPatch]
    [Route("DeactivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<DeactivateManyUsersCommandOutput>> DeactivateManyUsers([FromBody] int[] ids)
    {
        var command = new DeactivateManyUsersCommand { UserIds = ids };
        var output = pipeline.ExecuteCommand<DeactivateManyUsersCommand, DeactivateManyUsersCommandOutput>(command);

        return Resolve(output);
    }

    [HttpDelete]
    [Route("{id:int}/Delete")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<DeleteUserCommandOutput>> DeleteUser([FromRoute] int id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<DeleteUserCommand, DeleteUserCommandOutput>(command);

        return Resolve(output);
    }

    [HttpDelete]
    [Route("DeleteMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<DeleteManyUsersCommandOutput>> DeleteManyUsers(
        [FromBody] DeleteManyUsersCommand command)
    {
        var output = pipeline.ExecuteCommand<DeleteManyUsersCommand, DeleteManyUsersCommandOutput>(command);

        return Resolve(output);
    }
}
