namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.ValueObjects;

public sealed class UserService(IPasswordHasher passwordHasher, IUserRepository userRepository, IPlayerRepository playerRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEvents, IClashOfClansService clashOfClansService) : IUserService
{
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher _domainEvents = domainEvents;
    private readonly IClashOfClansService _clashOfClansService = clashOfClansService;

    public async Task<User> CreateUserAsync(string name, string email, string password, CancellationToken ct)
    {
        User? user = await _userRepository.FindByEmailAsync(Email.Create(email), ct);
        if (user is not null) throw new UserAlreadyExistsException($"User with email '{email}' already exists.");

        user = User.Create(name, email, _passwordHasher.Hash(password));
        await _userRepository.CreateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(user.PullDomainEvents(), ct);

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

        User? user = await _userRepository.FindByEmailAsync(emailObj, ct)
            ?? throw new UserNotFoundException($"User with email '{email}' was not found.");

        return user;
    }

    public async Task LinkPlayerAsync(Guid userId, string playerTag, string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new InvalidPlayerException("Player verification token cannot be empty.");

        User user = await GetUserAsync(userId, ct);
        Tag tag = Tag.Create(playerTag);

        bool isValidToken = await _clashOfClansService.VerifyPlayerTokenAsync(tag, token, ct);
        if (!isValidToken) throw new InvalidPlayerException("Player tag or verification token is invalid.");

        Player? player = await _playerRepository.FindByTagAsync(tag, ct);
        player ??= await _clashOfClansService.GetPlayerAsync(tag, ct);

        user.LinkPlayer(player);

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(player.PullDomainEvents().Concat(user.PullDomainEvents()), ct);
    }

    public async Task UnlinkPlayerAsync(Guid userId, CancellationToken ct)
    {
        User user = await GetUserAsync(userId, ct);

        user.UnlinkPlayer();

        await _userRepository.UpdateAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(user.PullDomainEvents(), ct);
    }
}
