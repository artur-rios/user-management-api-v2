using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Attributes;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Services;

namespace TechCraftsmen.Management.User.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthenticationController(AuthenticationService authenticationService) : BaseController
{
    [HttpPost]
    [Route("User")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<Authentication>> AuthenticateUser([FromBody] Credentials credentials)
    {
        return Resolve(authenticationService.AuthenticateUser(credentials)!);
    }
}
