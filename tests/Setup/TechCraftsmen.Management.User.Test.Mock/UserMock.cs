using Bogus;
using TechCraftsmen.Core.Util.Hashing;
using TechCraftsmen.Core.Util.Random;
using TechCraftsmen.Management.User.Domain;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Test.Mock;

public class UserMock
{
    public string MockPassword { get; }

    private readonly Faker<Domain.Aggregates.User> _userFaker;

    private UserMock()
    {
        MockPassword = CustomRandom.Text(new RandomStringOptions { Length = Constants.MinimumPasswordLength });

        var mockPasswordHash = Hash.NewFromText(MockPassword);

        _userFaker = new Faker<Domain.Aggregates.User>()
            .RuleFor(x => x.Id, f => f.Random.Int(1, 1000))
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, _ => mockPasswordHash.Value)
            .RuleFor(x => x.Salt, _ => mockPasswordHash.Salt)
            .RuleFor(x => x.RoleId, f => f.PickRandom((int)Roles.Admin, (int)Roles.Regular))
            .RuleFor(x => x.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(x => x.Active, _ => true);
    }

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

    public Domain.Aggregates.User Generate()
    {
        return _userFaker.Generate();
    }
}
