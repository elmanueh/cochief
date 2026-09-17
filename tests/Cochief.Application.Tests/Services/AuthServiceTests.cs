namespace Cochief.Application.Tests.Services;

using Cochief.Application.Exceptions;
using Cochief.Application.Services;
using Cochief.Application.Tests.Fixtures;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using Moq;

public sealed class AuthServiceTests : AutoMoqTest
{
    private static readonly DateTimeOffset Now = new(2026, 9, 15, 10, 0, 0, TimeSpan.Zero);

    private readonly Mock<IUserService> _users;
    private readonly Mock<IUserSessionRepository> _sessions;
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly Mock<IAuthTokenProvider> _tokenProvider;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IDomainEventDispatcher> _domainEvents;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _users = Freeze<IUserService>();
        _sessions = Freeze<IUserSessionRepository>();
        _passwordHasher = Freeze<IPasswordHasher>();
        _tokenProvider = Freeze<IAuthTokenProvider>();
        _unitOfWork = Freeze<IUnitOfWork>();
        _domainEvents = Freeze<IDomainEventDispatcher>();
        Inject<TimeProvider>(new StubTimeProvider(Now));
        _authService = Create<AuthService>();
    }

    [Fact]
    public async Task RegisterAsync_DelegatesToUserService()
    {
        User user = TestDataBuilder.BuildUser();
        _users
            .Setup(service => service.CreateUserAsync("Test User", "test@example.com", "password", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        User result = await _authService.RegisterAsync("Test User", "test@example.com", "password", TestContext.Current.CancellationToken);

        Assert.Same(user, result);
        _users.Verify(service => service.CreateUserAsync("Test User", "test@example.com", "password", TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ThrowsGenericAuthException()
    {
        _users
            .Setup(service => service.GetUserByEmailAsync("missing@example.com", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserNotFoundException("Internal detail"));

        Task Act() => _authService.LoginAsync("missing@example.com", "password", TestContext.Current.CancellationToken);

        AuthException exception = await Assert.ThrowsAsync<AuthException>(Act);
        Assert.Equal("Email or password is incorrect.", exception.Message);
        _passwordHasher.Verify(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsAndDoesNotCreateSession()
    {
        User user = TestDataBuilder.BuildUser();
        _users
            .Setup(service => service.GetUserByEmailAsync(user.Email.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(hasher => hasher.Verify("wrong-password", user.PasswordHash)).Returns(false);

        Task Act() => _authService.LoginAsync(user.Email.Value, "wrong-password", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<AuthException>(Act);
        _sessions.Verify(repository => repository.CreateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_CreatesSessionAndReturnsTokens()
    {
        User user = TestDataBuilder.BuildUser();
        AuthTokenPair tokens = TestDataBuilder.BuildTokenPair(Now);
        _users
            .Setup(service => service.GetUserByEmailAsync(user.Email.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(hasher => hasher.Verify("password", user.PasswordHash)).Returns(true);
        _tokenProvider.Setup(provider => provider.Issue(user.Id, It.IsAny<Guid>())).Returns(tokens);
        _tokenProvider.Setup(provider => provider.HashRefreshToken(tokens.RefreshToken)).Returns("refresh-hash");

        UserAuthentication result = await _authService.LoginAsync(user.Email.Value, "password", TestContext.Current.CancellationToken);

        Assert.Same(user, result.User);
        Assert.Same(tokens, result.Tokens);
        Assert.NotEqual(Guid.Empty, result.SessionId);
        _sessions.Verify(repository => repository.CreateAsync(
                It.Is<UserSession>(session =>
                    session.Id == result.SessionId &&
                    session.UserId == user.Id &&
                    session.RefreshTokenHash == "refresh-hash" &&
                    session.CreatedAt == Now),
                TestContext.Current.CancellationToken),
            Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEvents.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task RefreshAsync_WhenTokenIsEmpty_Throws(string? refreshToken)
    {
        Task Act() => _authService.RefreshAsync(refreshToken!, TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<AuthException>(Act);

        _sessions.Verify(repository => repository.FindByRefreshTokenHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WhenSessionDoesNotExist_Throws()
    {
        _tokenProvider.Setup(provider => provider.HashRefreshToken("unknown-token")).Returns("unknown-hash");
        _sessions
            .Setup(repository => repository.FindByRefreshTokenHashAsync("unknown-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSession?)null);

        Task Act() => _authService.RefreshAsync("unknown-token", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<AuthException>(Act);
    }

    [Fact]
    public async Task RefreshAsync_WhenSessionIsExpired_ThrowsWithoutIssuingTokens()
    {
        User user = TestDataBuilder.BuildUser();
        UserSession expiredSession = UserSession.Restore(Guid.NewGuid(), user.Id, "old-hash", Now.AddDays(-8), Now, null);
        _tokenProvider.Setup(provider => provider.HashRefreshToken("old-token")).Returns("old-hash");
        _sessions
            .Setup(repository => repository.FindByRefreshTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredSession);

        Task Act() => _authService.RefreshAsync("old-token", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<AuthException>(Act);
        _tokenProvider.Verify(provider => provider.Issue(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_WithActiveSession_RotatesSessionAndReturnsNewTokens()
    {
        User user = TestDataBuilder.BuildUser();
        UserSession session = UserSession.Restore(Guid.NewGuid(), user.Id, "old-hash", Now.AddDays(-1), Now.AddDays(1), null);
        AuthTokenPair tokens = TestDataBuilder.BuildTokenPair(Now);
        _tokenProvider.Setup(provider => provider.HashRefreshToken("old-token")).Returns("old-hash");
        _tokenProvider.Setup(provider => provider.HashRefreshToken(tokens.RefreshToken)).Returns("new-hash");
        _sessions
            .Setup(repository => repository.FindByRefreshTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _users.Setup(service => service.GetUserAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _tokenProvider.Setup(provider => provider.Issue(user.Id, session.Id)).Returns(tokens);

        UserAuthentication result = await _authService.RefreshAsync("old-token", TestContext.Current.CancellationToken);

        Assert.Same(tokens, result.Tokens);
        Assert.Equal("new-hash", session.RefreshTokenHash);
        Assert.Equal(tokens.RefreshTokenExpiresAt, session.ExpiresAt);
        _sessions.Verify(repository => repository.UpdateAsync(session, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEvents.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_WhenTokenIsEmpty_DoesNothing()
    {
        await _authService.LogoutAsync(null, TestContext.Current.CancellationToken);

        _tokenProvider.Verify(provider => provider.HashRefreshToken(It.IsAny<string>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_WhenSessionDoesNotExist_DoesNothing()
    {
        _tokenProvider.Setup(provider => provider.HashRefreshToken("unknown-token")).Returns("unknown-hash");
        _sessions
            .Setup(repository => repository.FindByRefreshTokenHashAsync("unknown-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserSession?)null);

        await _authService.LogoutAsync("unknown-token", TestContext.Current.CancellationToken);

        _sessions.Verify(repository => repository.UpdateAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_WhenSessionExists_RevokesAndPersistsSession()
    {
        User user = TestDataBuilder.BuildUser();
        UserSession session = UserSession.Restore(Guid.NewGuid(), user.Id, "refresh-hash", Now.AddDays(-1), Now.AddDays(1), null);
        _tokenProvider.Setup(provider => provider.HashRefreshToken("refresh-token")).Returns("refresh-hash");
        _sessions
            .Setup(repository => repository.FindByRefreshTokenHashAsync("refresh-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        await _authService.LogoutAsync("refresh-token", TestContext.Current.CancellationToken);

        Assert.Equal(Now, session.RevokedAt);
        _sessions.Verify(repository => repository.UpdateAsync(session, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEvents.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }
}
