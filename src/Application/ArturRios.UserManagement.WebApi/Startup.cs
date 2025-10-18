using ArturRios.Common.Web.Api.Configuration;
using ArturRios.Common.Web.Middleware;
using ArturRios.Common.Web.Security.Middleware;
using ArturRios.UserManagement.Data.Relational.Configuration;
using ArturRios.UserManagement.IoC.DependencyInjection;

namespace ArturRios.UserManagement.WebApi;

public class Startup(string[] args) : WebApiStartup(args)
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Startup>();

    private const int MaxDbConnectionRetries = 5;
    private const int DelayDbConnectionAttemptMilliseconds = 2000;

    public override void Build()
    {
        ConfigureLogging();

        Logger.LogInformation("Building web api on {EnvironmentEnvironmentName} environment", Builder.Environment.EnvironmentName);

        LoadConfiguration();

        Logger.LogInformation("Configuration loaded successfully");

        ConfigureWebApi();

        Logger.LogInformation("Web api configured successfully");

        AddDependencies();

        Logger.LogInformation("Dependencies added successfully");

        AddCustomInvalidModelStateResponse();
        UseSwaggerGen(jwtAuthentication: true);

        BuildApp();

        Logger.LogInformation("App built successfully");

        ConfigureApp();
        AddMiddlewares([typeof(ExceptionMiddleware), typeof(JwtMiddleware)]);
        UseSwagger();

        Logger.LogInformation("App configured successfully");

        StartServices();

        Logger.LogInformation("Services started successfully");
        Logger.LogInformation("Ready to run!");
    }

    private void ConfigureLogging()
    {
        Builder.Services.AddLogging();
    }

    public override void AddDependencies()
    {
        Builder.Services.AddCommandValidators();
        Builder.Services.AddCommands();
        Builder.Services.AddQueries();
        Builder.Services.AddRelationalContext();
        Builder.Services.AddRelationalRepositories();
        Builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    public override void ConfigureApp()
    {
        App.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        App.UseHttpsRedirection();
        App.UseRouting();
        App.UseAuthentication();
        App.UseAuthorization();
        App.MapControllers();
        App.UseDeveloperExceptionPage();
    }

    public override void ConfigureSecurity()
    {
        Builder.Services.AddAuthentication("Jwt").AddJwtBearer("Jwt");
        Builder.Services.AddAuthorization();
    }

    public override void ConfigureWebApi()
    {
        Builder.Services.AddControllers();
        Builder.Services.AddEndpointsApiExplorer();
    }

    public override void StartServices()
    {
        using var scope = App.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<RelationalDbInitializer>();
        var attempt = 0;

        while (true)
        {
            try
            {
                dbInitializer.InitializeAsync().GetAwaiter().GetResult();

                Logger.LogInformation("Database initialized successfully");

                break;
            }
            catch (Exception ex)
            {
                attempt++;

                Logger.LogError(ex, "Database initialization failed on attempt {Attempt}", attempt);
                Logger.LogError("Reason: {ExceptionMessage}", ex.Message);

                if (attempt >= MaxDbConnectionRetries)
                {
                    Logger.LogCritical("Max retry attempts reached. Unable to initialize database");

                    throw;
                }

                Logger.LogInformation("Waiting {DelayDbConnectionAttemptMilliseconds}ms before next attempt", DelayDbConnectionAttemptMilliseconds);

                Thread.Sleep(DelayDbConnectionAttemptMilliseconds);
            }
        }
    }
}
