using ArturRios.Common.Util.Hashing;
using ArturRios.Common.Util.Random;
using ArturRios.UserManagement.Domain;
using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;
using Bogus;

namespace ArturRios.UserManagement.Test.Mock;

public class UserMock
{
    private readonly Faker<User> _userFaker;

    private UserMock()
    {
        MockPassword = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var mockPasswordHash = Hash.NewFromText(MockPassword);

        _userFaker = new Faker<User>()
            .RuleFor(x => x.Id, f => f.Random.Int(1, 1000))
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, _ => mockPasswordHash.Value)
            .RuleFor(x => x.Salt, _ => mockPasswordHash.Salt)
            .RuleFor(x => x.RoleId, f => f.PickRandom((int)Roles.Admin, (int)Roles.Regular))
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.Active, _ => true);
    }

    public string MockPassword { get; }

    public static UserMock New => new();

    public UserMock WithId(int id)
    {
        _userFaker.RuleFor(x => x.Id, id);

        return this;
    }

    public UserMock WithNoId()
    {
        _userFaker.RuleFor(x => x.Id, 0);

        return this;
    }

    public UserMock WithEmail(string email)
    {
        _userFaker.RuleFor(x => x.Email, email);

        return this;
    }

    public UserMock WithName(string name)
    {
        _userFaker.RuleFor(x => x.Name, name);

        return this;
    }

    public UserMock WithRole(Roles role)
    {
        _userFaker.RuleFor(x => x.RoleId, _ => (int)role);

        return this;
    }

    public UserMock Active()
    {
        _userFaker.RuleFor(x => x.Active, _ => true);

        return this;
    }

    public UserMock Inactive()
    {
        _userFaker.RuleFor(x => x.Active, _ => false);

        return this;
    }

    public User Generate() => _userFaker.Generate();
}
