using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shortify.NET.API.SwaggerConfig
{
    internal class SwaggerConfigOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) 
        : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider = apiVersionDescriptionProvider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(desc.GroupName, new OpenApiInfo
                {
                    Title = "Shortify.NET API",
                    Version = desc.ApiVersion.ToString(),
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
            }
        }
    }
}
