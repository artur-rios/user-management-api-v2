namespace TechCraftsmen.Management.User.Data.Configuration;

public class RelationalDbContextOptions
{
    public string RelationalDatabase { get; init; } = Environment.GetEnvironmentVariable("RELATIONAL_DATABASE_CONNECTION_STRING") ?? string.Empty;
}
