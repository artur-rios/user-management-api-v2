namespace ArturRios.UserManagement.Data.Relational.Services;

public class HealthCheckOutput
{
    public bool Healthy { get; set; }
    public bool DatabaseConnectionHealthy { get; set; }
    public bool InitialAdminUserCreated { get; set; }
    public bool DefaultRegularUserCreated { get; set; }
    public bool DefaultTestUserCreated { get; set; }
}
