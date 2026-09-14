namespace Cochief.Api.Authentication;

using Cochief.Application.Exceptions;
using Cochief.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCochiefAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtOptions jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(subject, out Guid userId)
            ? userId
            : throw new AuthException("The authenticated user identifier is invalid.");
    }

    public static string GetBearerToken(this HttpRequest request)
    {
        if (!AuthenticationHeaderValue.TryParse(request.Headers.Authorization.ToString(), out AuthenticationHeaderValue? authorization) ||
            !string.Equals(authorization.Scheme, JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(authorization.Parameter))
        {
            throw new AuthException("A bearer token is required.");
        }

        return authorization.Parameter;
    }
}
