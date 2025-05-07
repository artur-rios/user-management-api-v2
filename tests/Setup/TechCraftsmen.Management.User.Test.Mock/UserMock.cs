using System.Text;
using Bogus;
using TechCraftsmen.Core.Validation;
using TechCraftsmen.Management.User.Domain;
using TechCraftsmen.Management.User.Domain.Enums;

namespace TechCraftsmen.Management.User.Test.Mock;

public class UserMock
{
    private readonly Faker<Domain.Aggregates.User> _userFaker;

    private UserMock()
    {
        _userFaker = new Faker<Domain.Aggregates.User>()
            .RuleFor(x => x.Id, f => f.Random.Int(1, 1000))
            .RuleFor(x => x.Name, f => f.Name.FullName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, _ => Encoding.UTF8.GetBytes(GenerateRandomPassword()))
            .RuleFor(x => x.Salt, f => Encoding.UTF8.GetBytes(f.Random.String(16)))
            .RuleFor(x => x.RoleId, f => f.PickRandom((int)Roles.Admin, (int)Roles.Regular))
            .RuleFor(x => x.CreatedAt, _ => DateTime.Now)
            .RuleFor(x => x.Active, _ => true);
    }

    public static UserMock New => new();

    public UserMock WithId(int id)
    {
        _userFaker.RuleFor(x => x.Id, id);

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

    private static string GenerateRandomPassword()
    {
        var random = new Random();
        var password = new StringBuilder();

        password.Append(CharacterCollection.LowerChars[random.Next(CharacterCollection.LowerChars.Length)]);
        password.Append(CharacterCollection.UpperChars[random.Next(CharacterCollection.UpperChars.Length)]);
        password.Append(CharacterCollection.Digits[random.Next(CharacterCollection.Digits.Length)]);

        for (var i = 3; i < Constants.MinimumPasswordLength; i++)
        {
            password.Append(CharacterCollection.AllChars[random.Next(CharacterCollection.AllChars.Length)]);
        }

        return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
    }
}
