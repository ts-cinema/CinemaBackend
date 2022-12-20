using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Services;
using Cinema.Backend.Service.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Template.Service.Configuration;
using Template.Service.Web;

namespace Template.Service
{
    /// <summary>
    /// Represents the startup logic for the web host.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialize a new instance of the Startup class.
        /// </summary>
        /// <param name="configuration">
        /// Represents a set of key:value configuration properties.
        /// </param>
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            // Cache the web host's configuration properties
            Program.Configuration = configuration;

            // Cache the web host's logger
            Program.Logger = logger;

            _configuration = configuration;
        }

        /// <summary>
        /// Configures all services used by the web host.
        /// </summary>
        /// <param name="services">
        /// A collection of services to add to the web host.
        /// </param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the web host.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddMongoDbStores<User, Role, Guid>(
                _configuration.GetMongoConnectionString(), _configuration.GetMongoDatabaseName()
             )
            .AddDefaultTokenProviders();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetTokenSigningKey())),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2), // allow for some drift in server time
                    ValidateAudience = false
                };
            });

            // Add the Web API controllers
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


            // Add CORs services
            services.AddCors(options =>
            {
                options.AddPolicy("Cinema", builder =>
                {
                    builder.WithOrigins("http://*.com", "http://localhost", "https://localhost")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // Check if the swagger endpoint should be published
            bool publishSwaggerEndpoint = Program.Configuration.GetSwaggerPublishEndpoint();
            if (publishSwaggerEndpoint)
            {
                // Register the Swagger generator
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Template API",
                        Version = "v1",
                        Description = "Template API."
                    });
                });
                services.AddSwaggerGenNewtonsoftSupport();
            }
        }

        /// <summary>
        /// Configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        /// An object used to configure the HTTP request pipelines
        /// </param>
        /// <param name="configuration">
        /// An object that represents the service's configuration.
        /// </param>
        /// <param name="environment">
        /// An object that represents information about the web hosting environment.
        /// </param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        public void Configure(IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Check if the swagger endpoint should be published
            bool publishSwaggerEndpoint = configuration.GetSwaggerPublishEndpoint();
            if (publishSwaggerEndpoint)
            {
                string prefix = $"swagger/{configuration.GetServiceName().ToLower()}";

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger(options =>
                {
                    options.RouteTemplate = $"{prefix}/{{documentname}}/swagger.json";
                });

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.) specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"/{prefix}/v1/swagger.json", "Entity API v1");
                    options.RoutePrefix = $"{prefix}";
                });
            }

            // Enable endpoint routing
            app.UseRouting();

            // Enable authentication 
            app.UseAuthentication();

            // Enable authorization
            app.UseAuthorization();
            
            // Enable API exception handling capabilities
            app.UseMiddleware(typeof(WebApiExceptionHandler));

            // Enable CORs
            app.UseCors("Cinema");

            // Executes the endpoint that was selected by routing.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeetUsersAndRoles(app, configuration);
        }

        private async void SeetUsersAndRoles(IApplicationBuilder app, IConfiguration configuration)
        {
            using IServiceScope serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();

            await DbHelper.SeedUserRoles(roleManager);
            await DbHelper.SeedAdminUser(userManager, configuration);
        }
    }
}
