using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Security.Middleware;
using ArturRios.UserManagement.Data.Configuration;
using ArturRios.UserManagement.WebApi.DependencyInjection;

namespace ArturRios.UserManagement.WebApi;

public class Startup(string[] args) : WebApiStartup(args)
{
    public override void Build()
    {
        LoadConfiguration();
        ConfigureServices();
        UseSwaggerGen(jwtAuthentication: true);

        BuildApp();
        
        ConfigureApp();
        AddMiddlewares([typeof(ExceptionMiddleware), typeof(JwtMiddleware)]);
        UseSwagger();
        StartServices();
    }

    public override void ConfigureServices()
    {
        Builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        Builder.Services.AddControllers();
        Builder.Services.AddLogging();
        Builder.Services.AddEndpointsApiExplorer();
        Builder.Services.AddRelationalContext();
        Builder.Services.AddJwtTokenConfiguration();
        Builder.Services.AddModelValidators();
        Builder.Services.AddRelationalRepositories();
        Builder.Services.AddServices();
        Builder.Services.AddAuthentication("Jwt").AddJwtBearer("Jwt");
        Builder.Services.AddAuthorization();
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

    public override void StartServices()
    {
        using var scope = Builder.Services.BuildServiceProvider().CreateScope();

        scope.ServiceProvider
            .GetRequiredService<RelationalDbInitializer>()
            .InitializeAsync()
            .GetAwaiter()
            .GetResult();
    }
}