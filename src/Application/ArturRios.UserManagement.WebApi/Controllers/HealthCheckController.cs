using ArturRios.Common.Output;
using ArturRios.Common.Web.AspNetCore;
using ArturRios.Common.Web.Http;
using ArturRios.Common.Web.Security.Attributes;
using ArturRios.UserManagement.Data.Relational.Services;
using ArturRios.UserManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class HealthCheckController(HealthService healthService) : Controller
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

    [HttpGet]
    [Route("Detailed")]
    [RoleRequirement((int)Roles.Admin)]
    public async Task<ActionResult<DataOutput<HealthCheckOutput?>>> Detailed()
    {
        var output = await healthService.GetHealthStatusAsync();

        var result = DataOutput<HealthCheckOutput?>.New
            .WithData(output)
            .WithMessage("Health check completed");

        return ResponseResolver.Resolve(result, HttpStatusCodes.Ok);
    }
}
