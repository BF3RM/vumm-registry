using System.Diagnostics.CodeAnalysis;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VUModManagerRegistry.Authentication;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.S3;

namespace VUModManagerRegistry
{
    [ExcludeFromCodeCoverage]
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
            
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = Configuration.GetSection("Limits")
                    .GetValue<long>("MaxRequestBodySize");
            });
            
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;
            });
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("RegistryDatabase"));
            });
            
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            // Common
            services.AddSingleton<ISystemTimeProvider, SystemTimeProvider>();
            
            // Storage
            services.AddS3Storage(Configuration.GetSection("S3Storage"));
            
            // AuthenticationScheme
            services
                .AddAuthentication(AccessTokenDefaults.AuthenticationScheme)
                .AddScheme<AccessTokenOptions, AccessTokenHandler>(AccessTokenDefaults.AuthenticationScheme, null);
            
            // Authorization
            services
                .AddAuthorization(options =>
                    options.AddPolicy("CanPublish", policy =>
                        policy.RequireClaim("TokenType", AccessTokenType.Publish.ToString())))
                .AddScoped<IAuthorizationHandler, ModAuthorizationHandler>()
                .AddScoped<IAuthorizationHandler, ModVersionAuthorizationHandler>();

            // Repositories
            services
                .AddScoped<IModRepository, ModRepository>()
                .AddScoped<IModVersionRepository, ModVersionRepository>()
                .AddScoped<IModUserPermissionRepository, ModUserPermissionRepository>()
                .AddScoped<IAccessTokenRepository, AccessTokenRepository>()
                .AddScoped<IUserRepository, UserRepository>();
            
            // Services
            services
                .AddScoped<IAccessTokenService, AccessTokenService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IModAuthorizationService, ModAuthorizationService>()
                .AddScoped<IModService, ModService>()
                .AddScoped<IModStorage, S3ModStorage>();
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
            
            MigrateDatabase(app);
        }

        private void MigrateDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            }
        }
    }
}