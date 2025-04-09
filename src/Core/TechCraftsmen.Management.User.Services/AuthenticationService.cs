using FluentValidation;
using TechCraftsmen.Core.Data;
using TechCraftsmen.Core.Extensions;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.Validation;
using TechCraftsmen.Core.WebApi.Security.Interfaces;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Domain.Filters;

namespace TechCraftsmen.Management.User.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IValidator<Credentials> _credentialsValidator;
    private readonly ICrudRepository<Domain.Aggregates.User> _userRepository;

    public AuthenticationService(IValidator<Credentials> credentialsValidator, ICrudRepository<Domain.Aggregates.User> userRepository)
    {
        _userRepository = userRepository;
        _credentialsValidator = credentialsValidator;
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
            return new DataOutput<Authentication>(default, ["Invalid credentials"], false);
        }
        
        var passwordHash = new Hash(user.Password, user.Salt);
        
        if (!passwordHash.TextMatches(credentials.Password))
        {
            return new DataOutput<Authentication>(default, ["Invalid credentials"], false);
        }
        
        // TODO: parametrize token configuration
        var jwtToken = new JwtToken(user.Id, new JwtTokenConfiguration(86400, "issuer", "audience", "secret"));

        var authentication = new Authentication(jwtToken.Token, true, jwtToken.CreatedAt, jwtToken.Expiration);
        
        return new DataOutput<Authentication>(authentication, ["User authenticated with success"], true);
    }

    public DataOutput<AuthenticatedUser> ValidateTokenAndGetUser(string token)
    {
        // TODO: parametrize secret
        var jwtToken = new JwtToken(token, "secret");
        var isValid = jwtToken.IsTokenValid().GetAwaiter().GetResult();
        
        if (!isValid)
        {
            return new DataOutput<AuthenticatedUser>(default, ["Invalid token"], false);
        }
        
        var userId = jwtToken.GetUserId();

        var user = _userRepository.GetById(userId!.Value);

        if (user is null)
        {
            return new DataOutput<AuthenticatedUser>(default, ["User not found"], false);
        }
        
        return new DataOutput<AuthenticatedUser>(new AuthenticatedUser(user.Id, user.RoleId), ["Auth token is valid", "User retrieved"], true);
    }
}
