using Microsoft.AspNetCore.Mvc;
using TechCraftsmen.Core.Output;
using TechCraftsmen.Core.WebApi;
using TechCraftsmen.Core.WebApi.Security.Middleware;
using TechCraftsmen.Management.User.WebApi.DependencyInjection;

namespace TechCraftsmen.Management.User.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddRelationalContext(builder.Configuration.GetSection("ConnectionStrings"));
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
