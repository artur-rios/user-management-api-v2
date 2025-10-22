using ArturRios.Common.Extensions;

namespace ArturRios.UserManagement.Test.Util;

public static class TestExtensions
{
    public static int ExtractIdFromDomainError(this string error) => int.Parse(error.Split(" ")[3].TrimChar(':'));
}
