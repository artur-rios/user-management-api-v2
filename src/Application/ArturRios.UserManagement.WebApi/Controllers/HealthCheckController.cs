using ArturRios.Common.Output;
using ArturRios.Common.Web.AspNetCore;
using ArturRios.Common.Web.Http;
using ArturRios.Common.Web.Security.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class HealthCheckController : Controller
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<DataOutput<string?>> HelloWorld()
    {
        var result = DataOutput<string?>.New
            .WithData("Hello world!")
            .WithMessage("User management Web API is ON");

        return ResponseResolver.Resolve(result, HttpStatusCodes.Ok);
    }
}
