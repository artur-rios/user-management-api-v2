using System.Net;
using ArturRios.Common.Configuration.Enums;
using ArturRios.Common.Test;
using ArturRios.Common.Test.Attributes;
using ArturRios.Common.Web.Security.Records;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Test.Fixture;
using ArturRios.UserManagement.Test.Mock;

namespace ArturRios.UserManagement.WebApi.Tests;

public class AuthenticationTests(DatabaseFixture fixture, EnvironmentType environment = EnvironmentType.Local)
    : WebApiTest<Program>(environment), IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private const string AuthenticationRoute = "/Authentication";

    private User _testUser = new();

    public Task InitializeAsync()
    {
        _testUser = fixture.CreateUsers().First();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }


    [FunctionalFact]
    public async Task Should_AuthenticateUser()
    {
        var credentials = CredentialsMock.New
            .WithEmail(_testUser.Email)
            .WithPassword(fixture.GetPassword(_testUser.Id))
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.OK, output.GetStatusCode());
        Assert.NotNull(output);
        CustomAssert.NotNullOrWhiteSpace(output.Data?.Token);
        Assert.True(output.Data?.Valid);
        Assert.True(output.Success);
    }

    [FunctionalFact]
    public async Task ShouldNot_AuthenticateUser_When_PasswordIsIncorrect()
    {
        var credentials = CredentialsMock.New
            .WithEmail(_testUser.Email)
            .WithPassword("wrong-password")
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }

    [FunctionalFact]
    public async Task ShouldNot_AuthenticateUser_When_EmailIsIncorrect()
    {
        var credentials = CredentialsMock.New
            .WithEmail("wrong@mail.com")
            .WithPassword(fixture.GetPassword(_testUser.Id))
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }

    [FunctionalFact]
    public async Task ShouldNot_AuthenticateUser_When_CredentialsAreIncorrect()
    {
        var credentials = CredentialsMock.New
            .WithEmail("wrong@mail.com")
            .WithPassword("wrong-password")
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }

    [FunctionalTheory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    public async Task ShouldNot_AuthenticateUser_WhenEmailIsInvalid(string? email)
    {
        var credentials = CredentialsMock.New
            .WithEmail(email!)
            .WithPassword(fixture.GetPassword(_testUser.Id))
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }

    [FunctionalTheory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public async Task ShouldNot_AuthenticateUser_When_PasswordIsInvalid(string? password)
    {
        var credentials = CredentialsMock.New
            .WithEmail(_testUser.Email)
            .WithPassword(password!)
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }

    [FunctionalTheory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData(" ", "")]
    [InlineData("invalid-email", "")]
    public async Task Should_Not_AuthenticateUser_When_CredentialsAreInvalid(string? email, string? password)
    {
        var credentials = CredentialsMock.New
            .WithEmail(email!)
            .WithPassword(password!)
            .Generate();

        var output = await Gateway.PostAsync<Authentication>(AuthenticationRoute, credentials);

        Assert.Equal(HttpStatusCode.BadRequest, output.GetStatusCode());
        Assert.NotNull(output);
        Assert.True(string.IsNullOrEmpty(output.Data?.Token));
        Assert.False(output.Success);
    }
}
