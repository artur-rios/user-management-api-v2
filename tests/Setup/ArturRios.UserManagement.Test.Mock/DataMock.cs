using ArturRios.UserManagement.Domain.Aggregates;
using ArturRios.UserManagement.Domain.Enums;

namespace ArturRios.UserManagement.Test.Mock;

public class DataMock
{
    public const string NonexistentEmail = "nonexistent@mail.com";
    public const string NonexistentPassword = "nonexistentpassword";

    public readonly List<User> ActiveUsers = [];
    private readonly List<User> _inactiveUsers = [];

    public readonly List<int> ActiveIds = [];
    public readonly List<int> InactiveIds = [];
    public readonly List<int> NonexistentIds = [];

    private const string TestEmailSuffix = "@mail.com";
    private int _activeUserCount = 5;
    private int _inactiveUserCount = 5;

    private List<int> AllIds => ActiveIds.Concat(InactiveIds).ToList();
    private readonly Dictionary<int, string> _generatedPasswords = new();

    public static DataMock New => new();

    public DataMock Build()
    {
        ActiveUsers.Clear();
        _inactiveUsers.Clear();
        ActiveIds.Clear();
        InactiveIds.Clear();
        NonexistentIds.Clear();

        GenerateIds();
        GenerateUsers();

        return this;
    }

    public DataMock WithActiveUsers(int count)
    {
        _activeUserCount = count;

        return this;
    }

    public DataMock WithInactiveUsers(int count)
    {
        _inactiveUserCount = count;

        return this;
    }

    public List<User> AllUsers => ActiveUsers.Concat(_inactiveUsers).ToList();

    public int ActiveUserId => ActiveUsers.First().Id;

    public string ActiveEmail => ActiveUsers.First().Email;

    public string ActivePassword => _generatedPasswords[ActiveUsers.First().Id];

    public int ActiveRoleId => ActiveUsers.First().RoleId;

    private void GenerateIds()
    {
        for (var i = 1; i <= _activeUserCount; i++)
        {
            ActiveIds.Add(i);
        }

        var startInactive = (ActiveIds.Count > 0 ? ActiveIds.Max() : 0) + 1;
        var endInactive = startInactive + Math.Max(0, _inactiveUserCount) - 1;

        for (var i = startInactive; i <= endInactive; i++)
        {
            InactiveIds.Add(i);
        }


        var allIds = AllIds;

        if (allIds.Count > 0)
        {
            var startNonexistent = allIds.Max() + 1;
            var endNonexistent = startNonexistent + allIds.Count - 1;

            for (var i = startNonexistent; i <= endNonexistent; i++)
            {
                NonexistentIds.Add(i);
            }
        }
    }

    private void GenerateUsers()
    {
        foreach (var id in ActiveIds)
        {
            var mock = UserMock.New.WithId(id).WithEmail($"active{id}{TestEmailSuffix}").WithRole(Roles.Regular)
                .Active();

            _generatedPasswords.Add(id, mock.MockPassword);

            ActiveUsers.Add(mock.Generate());
        }

        foreach (var id in InactiveIds)
        {
            var mock = UserMock.New.WithId(id).WithEmail($"inactive{id}{TestEmailSuffix}").WithRole(Roles.Regular)
                .Inactive();

            _generatedPasswords.Add(id, mock.MockPassword);

            _inactiveUsers.Add(mock.Generate());
        }
    }
}
