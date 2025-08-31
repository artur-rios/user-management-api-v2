using ArturRios.Common.Configuration;
using ArturRios.Common.Output;
using ArturRios.Common.WebApi;
using ArturRios.Common.WebApi.Security.Middleware;
using ArturRios.UserManagement.Data.Configuration;
using ArturRios.UserManagement.WebApi.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace ArturRios.UserManagement.WebApi
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var envFolder = Path.Combine(basePath, "Environments");
            var envFile = Path.Combine(envFolder, $".env.{environment.EnvironmentName.ToLower()}");
            var defaultEnvFile = Path.Combine(envFolder, $".env.{nameof(EnvironmentType.Local).ToLower()}");
            
            DotNetEnv.Env.Load(File.Exists(envFile) ? envFile : defaultEnvFile);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddControllers();
            services.AddLogging();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddRelationalContext();
            services.AddJwtTokenConfiguration(Configuration.GetSection("AuthenticationTokenSettings"));
            services.AddModelValidators();
            services.AddRelationalRepositories();
            services.AddServices();
            services.Configure<ApiBehaviorOptions>(options =>
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
            services.AddAuthentication("Jwt").AddJwtBearer("Jwt");
            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            serviceProvider.CreateScope().ServiceProvider
                .GetRequiredService<RelationalDbInitializer>()
                .InitializeAsync()
                .GetAwaiter()
                .GetResult();

            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
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
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
