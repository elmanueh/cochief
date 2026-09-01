namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;

public sealed class UserService(IPasswordHasher passwordHasher, IUserRepository userRepository, IUnitOfWork unitOfWork, IClashOfClansService clashOfClansService) : IUserService
{
    public async Task<User> CreateUserAsync(string name, string email, string password, CancellationToken ct)
    {
        User? user = await userRepository.FindByEmailAsync(Email.Create(email), ct);
        if (user is not null) throw new UserAlreadyExistsException($"User with email '{email}' already exists.");

        user = User.Create(name, email, passwordHasher.Hash(password));
        await userRepository.CreateAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken ct)
    {
        User user = await userRepository.GetByIdAsync(userId, ct);

        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        Email emailObj = Email.Create(email);

        User? user = await userRepository.FindByEmailAsync(emailObj, ct);
        if (user is null) throw new UserNotFoundException($"User with email '{email}' was not found.");

        return user;
    }

    public async Task LinkPlayerAsync(Guid userId, string playerTag, string token, CancellationToken ct)
    {
        User user = await GetUserAsync(userId, ct);
        Tag tag = Tag.Create(playerTag);

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidPlayerException("Player verification token cannot be empty.");
        }

        bool isValidToken = await clashOfClansService.VerifyPlayerTokenAsync(tag, token, ct);
        if (!isValidToken)
        {
            throw new InvalidPlayerException("Player tag or verification token is invalid.");
        }

        Player player = await clashOfClansService.GetPlayerAsync(tag, ct);
        user.LinkPlayer(player);

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
