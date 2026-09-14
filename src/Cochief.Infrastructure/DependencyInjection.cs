namespace Cochief.Infrastructure;

using Cochief.Application.Events;
using Cochief.Application.Services;
using Cochief.Domain.Ports;
using Cochief.Infrastructure.ClashOfClans.Configuration;
using Cochief.Infrastructure.ClashOfClans.Generated;
using Cochief.Infrastructure.ClashOfClans.Services;
using Cochief.Infrastructure.Persistence;
using Cochief.Infrastructure.Persistence.Repositories;
using Cochief.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, Argon2idPasswordHasher>();
        services.AddSingleton<IAuthTokenProvider, JwtTokenProvider>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClanService, ClanService>();

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining<CreateClanOnUserPlayerLinkedHandler>();

            string? licenseKey = configuration["MediatR:LicenseKey"];
            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                options.LicenseKey = licenseKey;
            }
        });

        services.AddPersistence(configuration);
        services.AddAuthenticationSecurity(configuration);
        services.AddClashOfClans(configuration);

        return services;
    }

    public static IServiceCollection AddAuthenticationSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "JWT issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "JWT audience is required.")
            .Validate(options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32, "JWT signing key must contain at least 32 bytes.")
            .Validate(options => options.AccessTokenMinutes > 0, "JWT access token lifetime must be greater than zero.")
            .Validate(options => options.RefreshTokenDays > 0, "Refresh token lifetime must be greater than zero.")
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddClashOfClans(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(ClashOfClansOptions.SectionName);
        string baseUrl = section["BaseUrl"] ?? ClashOfClansOptions.DefaultBaseUrl;

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? baseAddress))
        {
            throw new InvalidOperationException($"Configuration value '{ClashOfClansOptions.SectionName}:BaseUrl' must be an absolute URL.");
        }

        ClashOfClansOptions options = new()
        {
            BaseUrl = baseUrl,
            ApiToken = section["ApiToken"] ?? string.Empty
        };

        services.AddSingleton(options);
        services.AddTransient<ClashOfClansAuthenticationHandler>();
        services.AddTransient<IClashOfClansService, ClashOfClansService>();
        services.AddHttpClient<IClashOfClansApiClient, ClashOfClansApiClient>(client => client.BaseAddress = baseAddress)
            .AddHttpMessageHandler<ClashOfClansAuthenticationHandler>();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("PostgreSql")
            ?? throw new InvalidOperationException("Connection string 'PostgreSql' is not configured.");

        services.AddDbContext<CochiefDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IClanRepository, ClanRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
