using ArturRios.Common.Pipelines;
using ArturRios.Common.Security;
using ArturRios.Common.Validation;
using ArturRios.Common.Web.Api.Base;
using ArturRios.Common.Web.Api.Output;
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
    TokenProvider tokenProvider) : BaseController
{
    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    public ActionResult<WebApiOutput<Authentication?>> AuthenticateUser([FromBody] AuthenticateUserCommand command)
    {
        var validationResult = jwtTokenConfigurationValidator.Validate(jwtTokenConfiguration.Value);

        var output = WebApiOutput<Authentication?>.New;

        if (!validationResult.IsValid)
        {
            output
                .WithData(null)
                .WithError("JWT token configuration is invalid");

            return Resolve(output);
        }

        var commandOutput = pipeline.ExecuteCommand<AuthenticateUserCommand, AuthenticateUserCommandOutput>(command);

        output.WithMessages(commandOutput.Messages);

        if (!commandOutput.Success || commandOutput.Data is null)
        {
            output
                .WithData(null)
                .WithErrors(commandOutput.Errors);

            return Resolve(output);
        }

        var authentication =
            tokenProvider.Provide(new AuthenticatedUser(commandOutput.Data.Id, commandOutput.Data.RoleId),
                jwtTokenConfiguration.Value);

        output.WithData(authentication);

        return Resolve(output);
    }
}
