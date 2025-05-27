using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.Environment;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Middleware;
using TechCraftsmen.Management.User.Data.Configuration;
using TechCraftsmen.Management.User.WebApi.DependencyInjection;

namespace TechCraftsmen.Management.User.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var environment = builder.Environment.EnvironmentName;
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var envFolder = Path.Combine(basePath, "Environments");
        var envFile = Path.Combine(envFolder, $".env.{environment.ToLower()}");
        var defaultEnvFile = Path.Combine(envFolder, $".env.{nameof(EnvironmentType.Local).ToLower()}");
        
        DotNetEnv.Env.Load(File.Exists(envFile) ? envFile : defaultEnvFile);

        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddControllers();
        builder.Services.AddLogging();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRelationalContext();
        builder.Services.AddJwtTokenConfiguration(builder.Configuration.GetSection("AuthenticationTokenSettings"));
        builder.Services.AddModelValidators();
        builder.Services.AddRelationalRepositories();
        builder.Services.AddServices();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => $"Parameter: {e.Key} | Error: {e.Value?.Errors.First().ErrorMessage}").ToArray();

                DataOutput<string> output = new(string.Empty, errors, false);

                return new BadRequestObjectResult(output);
            };
        });

        var app = builder.Build();

        app.Services.CreateAsyncScope().ServiceProvider
            .GetRequiredService<RelationalDbInitializer>()
            .InitializeAsync()
            .GetAwaiter()
            .GetResult();

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}