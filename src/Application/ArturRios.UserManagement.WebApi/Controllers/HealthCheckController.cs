using ArturRios.Common.Web.Api.Base;
using ArturRios.Common.Web.Api.Output;
using ArturRios.Common.Web.Http;
using ArturRios.Common.Web.Security.Attributes;
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
        var result = WebApiOutput<string>.New
            .WithData("Hello world!")
            .WithMessage("User management Web API is ON")
            .WithHttpStatusCode(HttpStatusCodes.Ok);

        return Resolve(result);
    }
}
