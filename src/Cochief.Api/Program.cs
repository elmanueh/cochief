using Cochief.Api.Middleware;
using Cochief.Api.Presentation.Mappers;
using Cochief.Infrastructure;
using Cochief.Infrastructure.Persistence;
using DotNetEnv;

Env.NoClobber().TraversePath().Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(configuration =>
{
    configuration.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
}, typeof(PresentationMappingProfile));

builder.Services.AddInfrastructure(builder.Configuration);

WebApplication app = builder.Build();

await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    CochiefDbContext dbContext = scope.ServiceProvider.GetRequiredService<CochiefDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
