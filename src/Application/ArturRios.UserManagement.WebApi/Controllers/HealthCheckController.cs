using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Security.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class HealthCheckController : BaseController
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<string>> HelloWorld()
    {
        WebApiOutput<string> result = new("Hello world!", ["User management Web API is ON"], true, HttpStatusCodes.Ok);
            
        return Resolve(result);
    }
}
