using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Security.Attributes;
using ArturRios.Common.WebApi.Security.Interfaces;
using ArturRios.Common.WebApi.Security.Records;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

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
