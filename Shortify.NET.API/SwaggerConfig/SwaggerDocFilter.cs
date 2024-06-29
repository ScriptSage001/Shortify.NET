using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shortify.NET.API.SwaggerConfig
{
    public class SwaggerDocFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var desc in context.ApiDescriptions)
            {
                if (desc.ParameterDescriptions.Any(p => p.Name == "X-Api-Version" && p.Source.Id == "Header"))
                    swaggerDoc.Paths.Remove($"/{desc.RelativePath?.TrimEnd('/')}");
            }

            AddTags(swaggerDoc);

            swaggerDoc.Info.Description = "<h1>Shortify.NET API v1.0.0.0</h1>\n    " +
                                          "<p><strong>Welcome to the Shortify.NET API documentation.</strong> Shortify.NET is a robust application " +
                                          "designed to simplify and streamline the process of URL shortening, user management, and " +
                                          "application monitoring. Our platform provides a variety of endpoints to support essential " +
                                          "functions such as user authentication, OTP (One Time Password) management, and URL " +
                                          "shortening operations.</p>\n    " +
                                          "<h2>Features</h2>\n    <ul>\n      " +
                                          "<li><strong>Application Monitoring</strong> \ud83d\udcca: Access endpoints to check the " +
                                          "health status of the application, ensuring optimal performance and reliability.</li>\n      " +
                                          "<li><strong>Authentication</strong> \ud83d\udd11: Utilize endpoints for user registration, " +
                                          "login, password reset, and token management, ensuring secure access to your services.</li>\n      " +
                                          "<li><strong>OTP Management</strong> \ud83d\udd22: Manage One Time Password operations to " +
                                          "enhance security and user verification processes.</li>\n      " +
                                          "<li><strong>URL Shortening</strong> \u2702\ufe0f: Efficiently shorten URLs to make them more " +
                                          "manageable and shareable, without sacrificing functionality.</li>\n      " +
                                          "<li><strong>User Management</strong> \ud83d\udc64: Administer user-related operations, including " +
                                          "creating, updating, and deleting user accounts.</li>\n    </ul>\n    " +
                                          "<h2>Coming Soon</h2>\n    " +
                                          "<p>We are constantly improving Shortify.NET. In future updates, we plan to introduce URL hit " +
                                          "tracking to provide detailed analytics on how your shortened URLs are being accessed and utilized.</p>\n    " +
                                          "<p>For more information, visit " +
                                          "<a href=\"https://www.linkedin.com/in/kaustab-samanta-b513511a1\">Kaustab Samanta's Website</a> or " +
                                          "<a href=\"scriptsage001@gmail.com\">send an email</a>.</p>\n    " +
                                          "<p>Licensed under Shortify.NET License.</p>";
        }

        private static void AddTags(OpenApiDocument swaggerDoc)
        {
            var predefinedTags = new List<OpenApiTag>
            {
                new() 
                {
                    Name = "🔑 Authentication",
                    Description = "Provides authentication-related operations including user " +
                                  "registration, login, password reset, and token management."
                },
                new() 
                {
                    Name = "📊 Application Monitoring",
                    Description = "Provides endpoints for checking the health status of the application."
                },
                new() 
                {
                    Name = "🔢 OTP Management",
                    Description = "Provides endpoints for managing OTP (One Time Password) operations."
                },
                new() 
                {
                    Name = "\u2702\ufe0f URL Shortening",
                    Description = "Provides endpoints for URL shortening operations."
                },
                new() 
                {
                    Name = "\ud83d\udc64 User Management",
                    Description = "Provides endpoints for managing user-related operations."
                }
            };

            // Extract unique tags from the operations in the swagger document
            var operationTags = swaggerDoc.Paths
                .SelectMany(pathItem => pathItem.Value.Operations.Values)
                .SelectMany(operation => operation.Tags)
                .Select(tag => tag.Name)
                .Distinct();

            // Filter predefined tags based on the tags used in operations
            var tagsToAdd = predefinedTags
                .Where(predefinedTag => operationTags.Contains(predefinedTag.Name))
                .OrderBy(tag => tag.Name)
                .ToList();

            // Assign the filtered and sorted tags to the swagger document
            swaggerDoc.Tags = tagsToAdd;
        }
    }
}