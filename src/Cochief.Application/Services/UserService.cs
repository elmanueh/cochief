namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;

public sealed class UserService(IPasswordHasher passwordHasher, IUserRepository userRepository, IUnitOfWork unitOfWork, IClashOfClansService clashOfClansService) : IUserService
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClashOfClansService _clashOfClansService = clashOfClansService;

    public async Task<User> CreateUserAsync(string name, string email, string password, CancellationToken ct)
    {
        User? user = await _userRepository.FindByEmailAsync(Email.Create(email), ct);
        if (user is not null) throw new UserAlreadyExistsException($"User with email '{email}' already exists.");

        user = User.Create(name, email, _passwordHasher.Hash(password));
        await _userRepository.CreateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return user;
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken ct)
    {
        User user = await _userRepository.GetByIdAsync(userId, ct);

        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        Email emailObj = Email.Create(email);

        User? user = await _userRepository.FindByEmailAsync(emailObj, ct);
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

        bool isValidToken = await _clashOfClansService.VerifyPlayerTokenAsync(tag, token, ct);
        if (!isValidToken)
        {
            throw new InvalidPlayerException("Player tag or verification token is invalid.");
        }

        Player player = await _clashOfClansService.GetPlayerAsync(tag, ct);
        user.LinkPlayer(player);

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UnlinkPlayerAsync(Guid userId, CancellationToken ct)
    {
        User user = await GetUserAsync(userId, ct);

        user.UnlinkPlayer();

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
