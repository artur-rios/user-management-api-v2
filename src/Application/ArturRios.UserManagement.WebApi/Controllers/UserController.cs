using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Security.Attributes;
using ArturRios.UserManagement.Domain.Enums;
using ArturRios.UserManagement.Domain.Filters;
using ArturRios.UserManagement.Dto;
using ArturRios.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController(UserService userService) : BaseController
{
    [HttpPost]
    [Route("Create/Admin")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<int>> CreateUser([FromBody] UserDto userDto)
    {
        return Resolve(userService.CreateAdmin(userDto));
    }

    [HttpPost]
    [Route("Create/Regular")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<int>> CreateRegular([FromBody] UserDto userDto)
    {
        return Resolve(userService.CreateRegular(userDto));
    }

    [HttpGet]
    [Route("{id:int}")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UserDto>> GetUserById([FromRoute] int id)
    {
        return Resolve(userService.GetById(id)!);
    }

    [HttpGet]
    [Route("Filter")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<IList<UserDto>>> GetUsersByFilter([FromQuery] UserFilter filter)
    {
        return Resolve(userService.GetByFilter(filter)!);
    }

    [HttpGet]
    [Route("Filter/Multi")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<IList<UserDto>>> GetUsersByMultiFilter([FromQuery] UserMultiFilter filter)
    {
        return Resolve(userService.GetByMultiFilter(filter)!);
    }

    [HttpPut]
    [Route("Update")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UserDto>> UpdateUser([FromBody] UserDto userDto)
    {
        return Resolve(userService.Update(userDto));
    }

    [HttpPatch]
    [Route("Update/{id:int}/Role/{newRoleId:int}")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> ChangeUserRole([FromRoute] int id, [FromRoute] int newRoleId)
    {
        return Resolve(userService.ChangeRole(id, newRoleId), $"User with id {id} changed to role {(Roles)newRoleId} successfully");
    }

    [HttpPatch]
    [Route("{id:int}/Activate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> ActivateUser([FromRoute] int id)
    {
        return Resolve(userService.Activate(id), $"User with id {id} activated successfully");
    }

    [HttpPatch]
    [Route("ActivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> ActivateManyUsers([FromBody] int[] ids)
    {
        return Resolve(userService.ActivateMany(ids), "All users activated successfully");
    }

    [HttpPatch]
    [Route("{id:int}/Deactivate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeactivateUser([FromRoute] int id)
    {
        return Resolve(userService.Deactivate(id), $"User with id {id} deactivated successfully");
    }

    [HttpPatch]
    [Route("DeactivateMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeactivateManyUsers([FromBody] int[] ids)
    {
        return Resolve(userService.DeactivateMany(ids), "All users deactivated successfully");
    }

    [HttpDelete]
    [Route("{id:int}/Delete")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeleteUser([FromRoute] int id)
    {
        return Resolve(userService.Delete(id), $"User with id {id} deleted successfully");
    }

    [HttpDelete]
    [Route("DeleteMany")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeleteManyUsers([FromBody] int[] ids)
    {
        return Resolve(userService.DeleteMany(ids), "All users deleted successfully");
    }
}
