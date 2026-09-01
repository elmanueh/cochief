namespace Cochief.Application.Services;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;
using Cochief.Infrastructure.Persistence.Exceptions;

public sealed class UserService(IPasswordHasher passwordHasher, IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
{
    public async Task<User> CreateUserAsync(string name, string email, string password, CancellationToken ct)
    {
        User? user = await userRepository.FindByEmailAsync(Email.Create(email), ct);
        if (user is not null) throw new EntityNotFoundException($"User with email '{email}' already exists.");

        user = User.Create(name, email, passwordHasher.Hash(password));
        await userRepository.CreateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken ct)
    {
        User user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new EntityNotFoundException($"User '{userId}' was not found.");

        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        Email emailObj = Email.Create(email);

        User? user = await userRepository.FindByEmailAsync(emailObj, ct);
        if (user is null) throw new EntityNotFoundException($"User with email '{email}' was not found.");

        return user;
    }

    public async Task LinkPlayerAsync(Guid userId, string playerTag, string token, CancellationToken ct)
    {
        User user = await GetUserAsync(userId, ct);

        // TODO: Replace these provisional player data with The Clash API verification.
        user.LinkPlayer(Player.Create("Pending verification", playerTag, 1));

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UnlinkPlayerAsync(Guid userId, CancellationToken ct)
    {
        User user = await GetUserAsync(userId, ct);

        user.UnlinkPlayer();

        await userRepository.UpdateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
