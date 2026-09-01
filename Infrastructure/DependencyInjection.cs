namespace Cochief.Infrastructure;

using Cochief.Application.Services;
using Cochief.Domain.Ports;
using Cochief.Infrastructure.Persistence;
using Cochief.Infrastructure.Persistence.Repositories;
using Cochief.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, Argon2idPasswordHasher>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddPersistence(configuration);

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
