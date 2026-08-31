namespace Cochief.Infrastructure;

using Cochief.Application.Services;
using Cochief.Domain.Services;
using Cochief.Infrastructure.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, Argon2idPasswordHasher>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IAuthService, AuthService>();

        return services;
    }
}
