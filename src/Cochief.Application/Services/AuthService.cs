namespace Cochief.Application.Services;

using Cochief.Application.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;

public sealed class AuthService(IUserService userService, IUserSessionRepository userSessionRepository, IPasswordHasher passwordHasher, IAuthTokenProvider authTokenProvider, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEvents, TimeProvider timeProvider) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IUserSessionRepository _userSessionRepository = userSessionRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IAuthTokenProvider _authTokenProvider = authTokenProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher _domainEvents = domainEvents;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<User> RegisterAsync(string name, string email, string password, CancellationToken ct)
    {
        return await _userService.CreateUserAsync(name, email, password, ct);
    }

    public async Task<UserAuthentication> LoginAsync(string email, string password, CancellationToken ct)
    {
        User user;
        try
        {
            user = await _userService.GetUserByEmailAsync(email, ct);
        }
        catch (UserNotFoundException)
        {
            throw new AuthException("Email or password is incorrect.");
        }

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new AuthException("Email or password is incorrect.");
        }

        return await CreateSessionAsync(user, ct);
    }

    public async Task<UserAuthentication> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) throw new AuthException("The session is invalid or has expired.");

        string refreshTokenHash = _authTokenProvider.HashRefreshToken(refreshToken);
        UserSession? session = await _userSessionRepository.FindByRefreshTokenHashAsync(refreshTokenHash, ct);
        DateTimeOffset now = _timeProvider.GetUtcNow();

        if (session is null || !session.IsActive(now)) throw new AuthException("The session is invalid or has expired.");

        User user = await _userService.GetUserAsync(session.UserId, ct);
        AuthTokenPair tokens = _authTokenProvider.Issue(user.Id, session.Id);

        session.Rotate(_authTokenProvider.HashRefreshToken(tokens.RefreshToken), tokens.RefreshTokenExpiresAt, now);

        await _userSessionRepository.UpdateAsync(session, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(session.PullDomainEvents(), ct);

        return new UserAuthentication(user, session.Id, tokens);
    }

    public async Task LogoutAsync(string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return;

        string refreshTokenHash = _authTokenProvider.HashRefreshToken(refreshToken);
        UserSession? session = await _userSessionRepository.FindByRefreshTokenHashAsync(refreshTokenHash, ct);

        if (session is null) return;

        session.Revoke(_timeProvider.GetUtcNow());
        await _userSessionRepository.UpdateAsync(session, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(session.PullDomainEvents(), ct);
    }

    private async Task<UserAuthentication> CreateSessionAsync(User user, CancellationToken ct)
    {
        Guid sessionId = Guid.NewGuid();
        AuthTokenPair tokens = _authTokenProvider.Issue(user.Id, sessionId);
        UserSession session = UserSession.Create(user.Id, _authTokenProvider.HashRefreshToken(tokens.RefreshToken), _timeProvider.GetUtcNow(), tokens.RefreshTokenExpiresAt, sessionId);

        await _userSessionRepository.CreateAsync(session, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(session.PullDomainEvents(), ct);

        return new UserAuthentication(user, session.Id, tokens);
    }
}
