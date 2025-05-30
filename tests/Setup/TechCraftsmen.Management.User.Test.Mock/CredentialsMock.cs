﻿using Bogus;
using TechCraftsmen.Core.Util.Random;
using TechCraftsmen.Core.WebApi.Security.Records;
using TechCraftsmen.Management.User.Domain;

namespace TechCraftsmen.Management.User.Test.Mock;

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
