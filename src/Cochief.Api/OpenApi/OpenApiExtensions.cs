namespace Cochief.Api.OpenApi;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

public static class OpenApiExtensions
{
    private const string RefreshTokenScheme = "RefreshToken";

    public static IServiceCollection AddCochiefOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi("v1", options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(ProblemDetails) ||
                    context.JsonTypeInfo.Type == typeof(ValidationProblemDetails))
                {
                    schema.Properties ??= new Dictionary<string, IOpenApiSchema>();
                    schema.Properties["traceId"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String,
                        Description = "Identifier used to correlate the request with server logs."
                    };
                }

                return Task.CompletedTask;
            });

            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Cochief API",
                    Version = "v1",
                    Description = "API for managing Cochief users, linked Clash of Clans players and clans."
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                {
                    [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT access token."
                    },
                    [RefreshTokenScheme] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "Refresh token",
                        In = ParameterLocation.Header,
                        Description = "Refresh token issued for an authentication session."
                    }
                };

                document.Security ??= [];
                document.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
                });

                foreach (OpenApiOperation operation in document.Paths.Values
                    .Where(path => path.Operations is not null)
                    .SelectMany(path => path.Operations!.Values)
                    .Where(operation => operation.OperationId is "RefreshAuthentication" or "LogoutUser"))
                {
                    operation.Security =
                    [
                        new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecuritySchemeReference(RefreshTokenScheme, document)] = []
                        }
                    ];
                }

                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, _) =>
            {
                if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
                {
                    operation.Security = [];
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
