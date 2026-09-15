namespace Cochief.Api.OpenApi;

using Cochief.Api.Presentation.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

internal static class OpenApiExporter
{
    public static async Task ExportAsync()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(AuthController).Assembly);

        builder.Services.AddCochiefOpenApi();

        await using WebApplication app = builder.Build();
        app.MapControllers();

        string repositoryPath = FindRepositoryPath();
        string outputPath = Path.Combine(repositoryPath, "openapi", "cochief-api.json");
        string temporaryPath = $"{outputPath}.tmp";

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await using (StreamWriter writer = File.CreateText(temporaryPath))
        {
            IOpenApiDocumentProvider provider = app.Services.GetRequiredKeyedService<IOpenApiDocumentProvider>("v1");
            OpenApiDocument document = await provider.GetOpenApiDocumentAsync();
            OpenApiJsonWriter openApiWriter = new(writer);

            document.SerializeAs(OpenApiSpecVersion.OpenApi3_0, openApiWriter);
        }

        File.Move(temporaryPath, outputPath, true);
        Console.WriteLine($"OpenAPI specification written to '{outputPath}'.");
    }

    private static string FindRepositoryPath()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "cochief.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Could not locate the repository root.");
    }
}
