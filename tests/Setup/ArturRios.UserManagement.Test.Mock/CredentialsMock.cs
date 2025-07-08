using ArturRios.Common.Util.Random;
using ArturRios.Common.WebApi.Security.Records;
using ArturRios.UserManagement.Domain;
using Bogus;

namespace ArturRios.UserManagement.Test.Mock;

public class CredentialsMock
{
    private readonly Faker<Credentials> _faker;

    private CredentialsMock()
    {
        _faker = new Faker<Credentials>()
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.Password, _ => CustomRandom.Text(new RandomStringOptions
                { Length = Constants.MinimumPasswordLength }));
    }
    
    public static CredentialsMock New => new();
    
    public CredentialsMock WithEmail(string email)
    {
        _faker.RuleFor(x => x.Email, email);
        
        return this;
    }
    
    public CredentialsMock WithPassword(string password)
    {
        _faker.RuleFor(x => x.Password, password);
        
        return this;
    }
    
    public Credentials Generate()
    {
        return _faker.Generate();
    }
}
