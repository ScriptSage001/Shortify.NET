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
                Name = "Authentication APIs",
                Description = "Provides authentication-related operations including user " +
                              "registration, login, password reset, and token management."
            }
        };

        swaggerDoc.Tags = openApiTags.OrderBy(tag => tag.Name).ToList();
    }
}
