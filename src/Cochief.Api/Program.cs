using Cochief.Api.Authentication;
using Cochief.Api.Middleware;
using Cochief.Api.OpenApi;
using Cochief.Api.Presentation.Mappers;
using Cochief.Api.Workers;
using Cochief.Infrastructure;
using DotNetEnv;

if (args is ["openapi"])
{
    await OpenApiExporter.ExportAsync();
    return;
}

Env.NoClobber().TraversePath().Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCochiefAuthentication();
builder.Services.AddAutoMapper(configuration =>
{
    configuration.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
}, typeof(PresentationMappingProfile));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<ClanSynchronizationWorker>();

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
