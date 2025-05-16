using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Attributes;
using TechCraftsmen.Core.WebApi.Security.Interfaces;
using TechCraftsmen.Core.WebApi.Security.Records;

namespace TechCraftsmen.Management.User.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthenticationController(IAuthenticationService authenticationService) : BaseController
{
    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<Authentication>> AuthenticateUser([FromBody] Credentials credentials)
    {
        return Resolve(authenticationService.AuthenticateUser(credentials)!);
    }
}
