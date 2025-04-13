using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Attributes;
using TechCraftsmen.Management.User.Domain.Enums;
using TechCraftsmen.Management.User.Domain.Filters;
using TechCraftsmen.Management.User.Dto;
using TechCraftsmen.Management.User.Services;

namespace TechCraftsmen.Management.User.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController(UserService userService) : BaseController
{
    [HttpPost]
    [Route("Create")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<int>> CreateUser([FromBody] UserDto userDto)
    {
        return Resolve(userService.CreateUser(userDto));
    }
    
    [HttpGet]
    [Route("{id:int}")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UserDto>> GetUserById([FromRoute] int id)
    {
        return Resolve(userService.GetUserById(id)!);
    }
    
    [HttpGet]
    [Route("Filter")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<IList<UserDto>>> GetUsersByFilter([FromQuery] UserFilter filter)
    {
        return Resolve(userService.GetUsersByFilter(filter)!);
    }
    
    [HttpPut]
    [Route("Update")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<UserDto>> UpdateUser([FromBody] UserDto userDto)
    {
        return Resolve(userService.UpdateUser(userDto));
    }
    
    [HttpPatch]
    [Route("{id:int}/Activate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> ActivateUser([FromRoute] int id)
    {
        return Resolve(userService.ActivateUser(id), $"User with id {id} activated successfully");
    }
    
    [HttpPatch]
    [Route("{id:int}/Deactivate")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeactivateUser([FromRoute] int id)
    {
        return Resolve(userService.DeactivateUser(id), $"User with id {id} activated successfully");
    }
    
    [HttpDelete]
    [Route("{id:int}/Delete")]
    [RoleRequirement((int)Roles.Admin, (int)Roles.Test)]
    public ActionResult<WebApiOutput<string>> DeleteUser([FromRoute] int id)
    {
        return Resolve(userService.DeleteUser(id), $"User with id {id} deleted successfully");
    }
}
