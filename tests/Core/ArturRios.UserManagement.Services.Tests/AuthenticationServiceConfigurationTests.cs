// using ArturRios.Common.Security;
// using ArturRios.Common.Test.Attributes;
// using ArturRios.Common.Validation;
// using ArturRios.Common.WebApi.Security.Records;
// using ArturRios.Common.WebApi.Security.Validation;
// using ArturRios.UserManagement.Domain.Repositories;
// using ArturRios.UserManagement.Services.Exceptions;
// using Microsoft.Extensions.Options;
// using Moq;
//
// namespace ArturRios.UserManagement.Services.Tests;
//
// public class AuthenticationServiceConfigurationTests
// {
//     [UnitFact]
//     public void Should_CreateInstance()
//     {
//         IFluentValidator<Credentials> credentialsValidator = new CredentialsValidator();
//         var userRepository = new Mock<IUserRepository>().Object;
//         var jwtTokenConfiguration = new JwtTokenConfiguration(6000, "test-issuer", "test-audience", "test-secret");
//         IFluentValidator<JwtTokenConfiguration> jwtTokenConfigurationValidator = new JwtTokenConfigurationValidator();
//         var options = Options.Create(jwtTokenConfiguration);
//
//         var exception = Record.Exception(() =>
//             new AuthenticationService(credentialsValidator, userRepository, options, jwtTokenConfigurationValidator));
//
//         Assert.Null(exception);
//     }
//
//     [UnitFact]
//     public void ShouldNot_CreateInstance()
//     {
//         IFluentValidator<Credentials> credentialsValidator = new CredentialsValidator();
//         var userRepository = new Mock<IUserRepository>().Object;
//         var jwtTokenConfiguration = new JwtTokenConfiguration(6000, "test-issuer", "test-audience", string.Empty);
//         IFluentValidator<JwtTokenConfiguration> jwtTokenConfigurationValidator = new JwtTokenConfigurationValidator();
//         var options = Options.Create(jwtTokenConfiguration);
//
//         var exception = Record.Exception(() =>
//             new AuthenticationService(credentialsValidator, userRepository, options, jwtTokenConfigurationValidator));
//
//         Assert.NotNull(exception);
//         Assert.IsType<MissingConfigurationException>(exception);
//         Assert.Equal("Secret must not be empty", exception.Message);
//         Assert.Equal("Secret must not be empty", (exception as MissingConfigurationException)?.Messages.First());
//     }
// }
