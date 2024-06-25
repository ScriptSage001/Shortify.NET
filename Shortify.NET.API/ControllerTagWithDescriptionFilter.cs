using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shortify.NET.API;

public class ControllerTagWithDescriptionFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var openApiTags = new List<OpenApiTag>
        {
            new()
            {
                Name = "Authentication",
                Description = "Provides authentication-related operations including user " +
                              "registration, login, password reset, and token management."
            },
            new()
            {
                Name = "Application Monitoring",
                Description = "Provides endpoints for checking the health status of the application."
            },
            new()
            {
                Name = "OTP Management", 
                Description = "Provides endpoints for managing OTP (One Time Password) operations."
            },
            new()
            {
                Name = "URL Shortening", 
                Description = "Provides endpoints for URL shortening operations."
            },
            new()
            {
                Name = "User Management", 
                Description = "Provides endpoints for managing user-related operations."
            }
        };

        swaggerDoc.Tags = openApiTags.OrderBy(tag => tag.Name).ToList();
    }
}
