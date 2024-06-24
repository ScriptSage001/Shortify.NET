using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Shortify.NET.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomAuthentication(configuration);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            
            return services;
        }

        private static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                configuration.GetSection("AppSettings:Secret").Value!)),
                        ValidateIssuer = true,
                        ValidIssuer = configuration.GetSection("AppSettings:Issuer").Value,
                        ValidateAudience = false
                    });
        }
        
        private static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(swag =>
            {
                swag.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Shortify.NET API",
                    Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
                    Description = "Rest APIs Documentations for the Shortify.NET application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Kaustab Samanta",
                        Email = "scriptsage001@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/kaustab-samanta-b513511a1")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Shortify.NET, Licence",
                        Url = new Uri("https://tempuri.org/license")
                    }
                });

                swag.CustomSchemaIds(x => x.FullName);
                swag.ResolveConflictingActions(x => x.FirstOrDefault());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swag.IncludeXmlComments(xmlPath);
                
                swag.DocumentFilter<ControllerTagWithDescriptionFilter>();
            });
        }
    }
}
