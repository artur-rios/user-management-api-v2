using ArturRios.Common.Output;
using ArturRios.Common.Pipelines;
using ArturRios.Common.Security;
using ArturRios.Common.Validation;
using ArturRios.Common.Web.AspNetCore;
using ArturRios.Common.Web.Http;
using ArturRios.Common.Web.Security.Attributes;
using ArturRios.Common.Web.Security.Providers;
using ArturRios.Common.Web.Security.Records;
using ArturRios.UserManagement.Command.Commands;
using ArturRios.UserManagement.Command.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ArturRios.UserManagement.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AuthenticationController(
    Pipeline pipeline,
    IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
    IFluentValidator<JwtTokenConfiguration> jwtTokenConfigurationValidator,
    TokenProvider tokenProvider) : Controller
{
    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<DataOutput<Authentication?>> AuthenticateUser([FromBody] AuthenticateUserCommand command)
    {
        var validationResult = jwtTokenConfigurationValidator.Validate(jwtTokenConfiguration.Value);

        var output = DataOutput<Authentication?>.New;

        if (!validationResult.IsValid)
        {
            output
                .WithData(null)
                .WithError("JWT token configuration is invalid");

            return ResponseResolver.Resolve(output, HttpStatusCodes.InternalServerError);
        }

        var commandOutput = pipeline.ExecuteCommand<AuthenticateUserCommand, AuthenticateUserCommandOutput>(command);

        output.WithMessages(commandOutput.Messages);

        if (!commandOutput.Success || commandOutput.Data is null)
        {
            output
                .WithData(null)
                .WithErrors(commandOutput.Errors);

            return ResponseResolver.Resolve(output, HttpStatusCodes.Unauthorized);
        }

        var authentication =
            tokenProvider.Provide(new AuthenticatedUser(commandOutput.Data.Id, commandOutput.Data.RoleId),
                jwtTokenConfiguration.Value);

        output.WithData(authentication);

        return ResponseResolver.Resolve(output, HttpStatusCodes.Ok);
    }
}
