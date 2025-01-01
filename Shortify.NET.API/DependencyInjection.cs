using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shortify.NET.API.Helpers;
using Shortify.NET.API.SwaggerConfig;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shortify.NET.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomApiVersioning();
            services.AddCustomAuthentication(configuration);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            services.AddCorsPolicy(configuration);
            services.AddRateLimiting(configuration);
            
            return services;
        }

        private static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(), 
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
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
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    });
        }
        
        private static void AddSwagger(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>();

            services.AddSwaggerGen(swag =>
            {
                swag.CustomSchemaIds(x => x.FullName);
                swag.ResolveConflictingActions(x => x.FirstOrDefault());

                swag.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                
                swag.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swag.IncludeXmlComments(xmlPath);
                
                swag.DocumentFilter<SwaggerDocFilter>();
            });
        }
        
        private static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowShortifyUI", policy =>
                {
                    var origin = configuration.GetSection("AllowedClients").Get<string>();
                    if (origin is not null)
                    {
                        var origins = origin.Split(",");
                        policy
                            .WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    else
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                });
            });
        }

        private static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRateLimiter(options =>
            {
                options.OnRejected = (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
                    return new ValueTask(); 
                };
                
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var remoteIpAddress = context.Connection.RemoteIpAddress;

                    if (IPAddress.IsLoopback(remoteIpAddress!))
                        return RateLimitPartition.GetNoLimiter(IPAddress.Loopback.ToString());
                    
                    var rateLimiterOptions = configuration
                                                .GetSection("RateLimiterOptions")
                                                .Get<RateLimiterOptions>();
                    
                    if (rateLimiterOptions is not null)
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(
                            remoteIpAddress?.ToString()!, 
                            _ =>
                                new SlidingWindowRateLimiterOptions
                                {
                                    PermitLimit = rateLimiterOptions.PermitLimit,
                                    Window = TimeSpan.FromSeconds(rateLimiterOptions.WindowInSeconds),
                                    SegmentsPerWindow = rateLimiterOptions.SegmentsPerWindow,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = rateLimiterOptions.QueueLimit
                                });
                    }
                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback.ToString());
                });
            });
        }
    }
}
