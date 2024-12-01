using Asp.Versioning.ApiExplorer;
using Shortify.NET.API;
using Shortify.NET.Application;
using Shortify.NET.Common;
using Shortify.NET.Infrastructure;
using Shortify.NET.Persistence;


var builder = WebApplication.CreateBuilder(args);

{
    builder.Services
                .AddApi(builder.Configuration)
                .AddShortifyNetCommon(Shortify.NET.Application.AssemblyReference.Assembly)
                .AddPersistence(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApplication(builder.Configuration);
}

{
    var app = builder.Build();

    app.UseCors("AllowShortifyUI");
    
    #region Swagger Config

    var apiVersionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(swag => 
    {
        foreach (var desc in apiVersionDescProvider.ApiVersionDescriptions)
        {
            swag.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                $"Shortify.NET API - {desc.GroupName.ToUpper()}");
        }

        swag.InjectStylesheet("../swagger-ui/shortify-theme.css");
        swag.InjectJavascript("../swagger-ui/shortify-theme.js");
        
        swag.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        swag.EnableDeepLinking();
        swag.DisplayRequestDuration();
        swag.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        swag.EnableFilter();
        swag.ShowExtensions();
        swag.DocumentTitle = "Shortify.NET API Documentations";
    });

    #endregion

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseExceptionHandler(_ => { });
    
    app.MapControllers();
    app.Run();
}
