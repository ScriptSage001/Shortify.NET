using Shortify.NET.API;
using Shortify.NET.Common;
using Shortify.NET.Persistence;
using Shortify.NET.Infrastructure;
using Shortify.NET.Applicaion;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

{
    builder.Services
                .AddApi(builder.Configuration)
                .AddShortifyNetCommon(Shortify.NET.Applicaion.AssemblyReference.Assembly)
                .AddPersistence(builder.Configuration)
                .AddInfrastructure(builder.Configuration)
                .AddApplication(builder.Configuration);
}

{
    var app = builder.Build();

    #region Swagger Config

    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(swag => 
    {
        swag.InjectStylesheet("../swagger-ui/shortify-theme.css");
        swag.InjectJavascript("../swagger-ui/shortify-theme.js");

        swag.SwaggerEndpoint(
                    "../swagger/v1/swagger.json", 
                    $"Shortify.NET API V{Assembly.GetExecutingAssembly().GetName().Version}");
        
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
