namespace cochief;

using Cochief.Infrastructure;
using Cochief.Infrastructure.Middleware;
using Cochief.Infrastructure.Presentation.Mappers;

public partial class Program
{
    protected Program() { }

    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(configuration =>
        {
            configuration.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"];
        }, typeof(PresentationMappingProfile));

        builder.Services.AddInfrastructure(builder.Configuration);

        WebApplication app = builder.Build();

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
