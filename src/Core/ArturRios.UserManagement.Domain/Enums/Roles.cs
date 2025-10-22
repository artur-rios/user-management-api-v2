using System.ComponentModel;

namespace ArturRios.UserManagement.Domain.Enums;

public enum Roles
{
    [Description("User with all privileges")]
    Admin = 1,

    [Description("Regular system user with limited privileges")]
    Regular = 2,

    [Description("User for test purposes")]
    Test = 3
}
