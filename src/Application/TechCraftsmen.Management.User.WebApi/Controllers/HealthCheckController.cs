using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Attributes;

namespace TechCraftsmen.Management.User.WebApi.Controllers;

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
