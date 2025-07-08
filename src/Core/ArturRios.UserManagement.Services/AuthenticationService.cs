using ArturRios.Common.Extensions;
using ArturRios.Common.Output;
using ArturRios.Common.Security;
using ArturRios.Common.Util.Hashing;
using ArturRios.Common.WebApi.Security.Interfaces;
using ArturRios.Common.WebApi.Security.Records;
using ArturRios.UserManagement.Domain.Filters;
using ArturRios.UserManagement.Domain.Interfaces;
using ArturRios.UserManagement.Services.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace ArturRios.UserManagement.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IValidator<Credentials> _credentialsValidator;
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenConfiguration _jwtTokenConfiguration;
    private readonly IValidator<JwtTokenConfiguration> _jwtTokenConfigurationValidator;

    public AuthenticationService(IValidator<Credentials> credentialsValidator, IUserRepository userRepository, IOptions<JwtTokenConfiguration> jwtTokenConfiguration, IValidator<JwtTokenConfiguration> jwtTokenConfigurationValidator)
    {
        _credentialsValidator = credentialsValidator;
        _userRepository = userRepository;
        _jwtTokenConfiguration = jwtTokenConfiguration.Value;
        _jwtTokenConfigurationValidator = jwtTokenConfigurationValidator;
        
        ValidateTokenConfigAndThrow();
    }
    
    public DataOutput<Authentication> AuthenticateUser(Credentials credentials)
    {
        var validationResult = _credentialsValidator.Validate(credentials);

        if (!validationResult.IsValid)
        {
            return validationResult.ToDataOutput<Authentication>();
        }
        
        var filter = new UserFilter { Email = credentials.Email };
        var user = _userRepository.GetByFilter(filter).FirstOrDefault();
        
        if (user == null)
        {
            return new DataOutput<Authentication>(null, ["Invalid credentials"], false);
        }

        var passwordHash = Hash.NewFromBytes(value: user.Password, salt: user.Salt);
        
        if (!passwordHash.TextMatches(credentials.Password))
        {
            return new DataOutput<Authentication>(null, ["Invalid credentials"], false);
        }
        
        var jwtToken = new JwtToken(user.Id, _jwtTokenConfiguration);

        var authentication = new Authentication(jwtToken.Token, true, jwtToken.CreatedAt, jwtToken.Expiration);
        
        return new DataOutput<Authentication>(authentication, ["User authenticated with success"], true);
    }

    public DataOutput<AuthenticatedUser> ValidateTokenAndGetUser(string token)
    {
        var jwtToken = new JwtToken(token, _jwtTokenConfiguration.Secret);
        var isValid = jwtToken.IsTokenValid().GetAwaiter().GetResult();
        
        if (!isValid)
        {
            return new DataOutput<AuthenticatedUser>(null, ["Invalid token"], false);
        }
        
        var userId = jwtToken.GetUserId();

        var user = _userRepository.GetById(userId!.Value);

        return user is null ? new DataOutput<AuthenticatedUser>(null, ["User not found"], false) : new DataOutput<AuthenticatedUser>(new AuthenticatedUser(user.Id, user.RoleId), ["Auth token is valid", "User retrieved"], true);
    }
    
    private void ValidateTokenConfigAndThrow()
    {
        var validationResult = _jwtTokenConfigurationValidator.Validate(_jwtTokenConfiguration);

        if (validationResult.IsValid)
        {
            return;
        }

        var validationErrors = validationResult.Errors.Select(vf => vf.ErrorMessage).ToArray();

        throw new MissingConfigurationException(validationErrors);
    }
}
