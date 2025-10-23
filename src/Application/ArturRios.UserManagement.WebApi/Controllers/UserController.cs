using ArturRios.Common.Output;
using ArturRios.Common.Pipelines;
using ArturRios.Common.Web.AspNetCore;
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
public class UserController(Pipeline pipeline) : Controller
{
    [HttpPost]
    [Route("Create/Admin")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<CreateUserCommandOutput?>> CreateAdminUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Admin;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPost]
    [Route("Create/Regular")]
    [AllowAnonymous]
    public ActionResult<DataOutput<CreateUserCommandOutput?>> CreateRegularUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Regular;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPost]
    [Route("Create/Test")]
    [AllowAnonymous]
    public ActionResult<DataOutput<CreateUserCommandOutput?>> CreateTestUser([FromBody] CreateUserCommand command)
    {
        command.RoleId = (int)Roles.Test;

        var output = pipeline.ExecuteCommand<CreateUserCommand, CreateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpGet]
    [Route("{id:int}")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<UserQueryOutput?>> GetUserById([FromRoute] int id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var output = pipeline.ExecuteSingleQuery<GetUserByIdQuery, UserQueryOutput>(query);

        return ResponseResolver.Resolve(output);
    }

    [HttpGet]
    [Route("")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<UserQueryOutput?>> GetUserByEmail([FromQuery] GetUserByEmailQuery query)
    {
        var output = pipeline.ExecuteSingleQuery<GetUserByEmailQuery, UserQueryOutput>(query);

        return ResponseResolver.Resolve(output);
    }

    [HttpGet]
    [Route("Filter")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<PaginatedOutput<UserQueryOutput>> GetUsersByFilter(
        [FromQuery] GetUsersByFilterQuery query)
    {
        var output = pipeline.ExecuteQuery<GetUsersByFilterQuery, UserQueryOutput>(query);

        return ResponseResolver.Resolve(output);
    }

    [HttpGet]
    [Route("Filter/Multi")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<PaginatedOutput<UserQueryOutput>> GetUsersByMultiFilter(
        [FromQuery] GetUsersByMultiFilterQuery query)
    {
        var output = pipeline.ExecuteQuery<GetUsersByMultiFilterQuery, UserQueryOutput>(query);

        return ResponseResolver.Resolve(output);
    }

    [HttpPut]
    [Route("Update")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<UpdateUserCommandOutput?>> UpdateUser([FromBody] UpdateUserCommand command)
    {
        var output = pipeline.ExecuteCommand<UpdateUserCommand, UpdateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPatch]
    [Route("Update/Role")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<UpdateUserRoleCommandOutput?>> UpdateUserRole(
        [FromBody] UpdateUserRoleCommand command)
    {
        var output = pipeline.ExecuteCommand<UpdateUserRoleCommand, UpdateUserRoleCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPatch]
    [Route("{id:int}/Activate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<ActivateUserCommandOutput?>> ActivateUser([FromRoute] int id)
    {
        var command = new ActivateUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<ActivateUserCommand, ActivateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPatch]
    [Route("ActivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<ActivateManyUsersCommandOutput?>> ActivateManyUsers([FromBody] int[] ids)
    {
        var command = new ActivateManyUsersCommand { UserIds = ids };
        var output = pipeline.ExecuteCommand<ActivateManyUsersCommand, ActivateManyUsersCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPatch]
    [Route("{id:int}/Deactivate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<DeactivateUserCommandOutput?>> DeactivateUser([FromRoute] int id)
    {
        var command = new DeactivateUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<DeactivateUserCommand, DeactivateUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpPatch]
    [Route("DeactivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<DeactivateManyUsersCommandOutput?>> DeactivateManyUsers([FromBody] int[] ids)
    {
        var command = new DeactivateManyUsersCommand { UserIds = ids };
        var output = pipeline.ExecuteCommand<DeactivateManyUsersCommand, DeactivateManyUsersCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpDelete]
    [Route("{id:int}/Delete")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<DeleteUserCommandOutput?>> DeleteUser([FromRoute] int id)
    {
        var command = new DeleteUserCommand { UserId = id };
        var output = pipeline.ExecuteCommand<DeleteUserCommand, DeleteUserCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }

    [HttpDelete]
    [Route("DeleteMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<DataOutput<DeleteManyUsersCommandOutput?>> DeleteManyUsers(
        [FromBody] DeleteManyUsersCommand command)
    {
        var output = pipeline.ExecuteCommand<DeleteManyUsersCommand, DeleteManyUsersCommandOutput>(command);

        return ResponseResolver.Resolve(output);
    }
}
