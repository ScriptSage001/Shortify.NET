using Shortify.NET.API;
using Shortify.NET.Common;
using Shortify.NET.Persistence;
using Shortify.NET.Infrastructure;
using Shortify.NET.Applicaion;


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

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Shortify.NET API V1"));
    //}

    app.UseSwagger();
    app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "Shortify.NET API V1"));

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseExceptionHandler(_ => { });

    app.MapControllers();
    app.Run();
}
