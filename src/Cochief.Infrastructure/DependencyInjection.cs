namespace Cochief.Infrastructure;

using Cochief.Application.Services;
using Cochief.Domain.Ports;
using Cochief.Infrastructure.ClashOfClans;
using Cochief.Infrastructure.ClashOfClans.Generated;
using Cochief.Infrastructure.ClashOfClans.Services;
using Cochief.Infrastructure.Persistence;
using Cochief.Infrastructure.Persistence.Repositories;
using Cochief.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, Argon2idPasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddPersistence(configuration);
        services.AddClashOfClans(configuration);

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
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
