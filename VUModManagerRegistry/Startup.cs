using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VUModManagerRegistry.Authentication;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.IgnoreNullValues = true;
                options.PropertyNameCaseInsensitive = true;
            });
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("RegistryDatabase"));
            });
            
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            // AuthenticationScheme
            services
                .AddAuthentication(AccessTokenDefaults.AuthenticationScheme)
                .AddScheme<AccessTokenOptions, AccessTokenHandler>(AccessTokenDefaults.AuthenticationScheme, null);

            // Repositories
            services
                .AddScoped<IAccessTokenRepository, AccessTokenRepository>()
                .AddScoped<IUserRepository, UserRepository>();
            
            // Services
            services
                .AddScoped<IAccessTokenService, AccessTokenService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IModService, ModService>()
                .AddSingleton<IModUploadService, ModUploadService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}