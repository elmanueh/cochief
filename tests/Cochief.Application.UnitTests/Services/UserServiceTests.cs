namespace Cochief.Application.UnitTests.Services;

using Cochief.Application.Exceptions;
using Cochief.Application.Services;
using Cochief.Application.UnitTests.Fixtures;
using Cochief.Domain.Exceptions;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;
using Moq;

public sealed class UserServiceTests : AutoMoqTest
{
    private readonly Mock<IPasswordHasher> _passwordHasher;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IPlayerRepository> _playerRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcher;
    private readonly Mock<IClashOfClansService> _clashOfClansService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _passwordHasher = Freeze<IPasswordHasher>();
        _userRepository = Freeze<IUserRepository>();
        _playerRepository = Freeze<IPlayerRepository>();
        _unitOfWork = Freeze<IUnitOfWork>();
        _domainEventDispatcher = Freeze<IDomainEventDispatcher>();
        _clashOfClansService = Freeze<IClashOfClansService>();
        _userService = Create<UserService>();
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailAlreadyExists_ThrowsAndDoesNotPersist()
    {
        User user = TestDataBuilder.BuildUser();
        _userRepository
            .Setup(repository => repository.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        Task Act() => _userService.CreateUserAsync("Another User", user.Email.Value, "password", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<UserAlreadyExistsException>(Act);
        _userRepository.Verify(repository => repository.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_HashesPersistsAndDispatchesEvents()
    {
        _passwordHasher.Setup(hasher => hasher.Hash("password")).Returns("hashed-password");
        _userRepository
            .Setup(repository => repository.FindByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        User result = await _userService.CreateUserAsync("  New User  ", "new@example.com", "password", TestContext.Current.CancellationToken);

        Assert.Equal("New User", result.Name);
        Assert.Equal("new@example.com", result.Email.Value);
        Assert.Equal("hashed-password", result.PasswordHash);
        _passwordHasher.Verify(hasher => hasher.Hash("password"), Times.Once);
        _userRepository.Verify(repository => repository.CreateAsync(result, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_WhenUserDoesNotExist_Throws()
    {
        _userRepository
            .Setup(repository => repository.FindByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        Task Act() => _userService.GetUserByEmailAsync("missing@example.com", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task LinkPlayerAsync_WhenTokenIsEmpty_ThrowsBeforeLoadingUser()
    {
        Task Act() => _userService.LinkPlayerAsync(Guid.NewGuid(), "#PLAYER1", "", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidPlayerException>(Act);
        _userRepository.Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LinkPlayerAsync_WhenVerificationFails_ThrowsAndDoesNotPersist()
    {
        User user = TestDataBuilder.BuildUser();
        _userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clashOfClansService
            .Setup(service => service.VerifyPlayerTokenAsync(It.IsAny<Tag>(), "bad-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Task Act() => _userService.LinkPlayerAsync(user.Id, "#PLAYER1", "bad-token", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidPlayerException>(Act);
        _userRepository.Verify(repository => repository.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LinkPlayerAsync_WhenPlayerExists_LinksWithoutCreatingPlayer()
    {
        User user = TestDataBuilder.BuildUser();
        Player player = TestDataBuilder.BuildPlayer();
        _userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clashOfClansService
            .Setup(service => service.VerifyPlayerTokenAsync(player.Tag, "valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _playerRepository
            .Setup(repository => repository.FindByTagAsync(player.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        await _userService.LinkPlayerAsync(user.Id, player.Tag.Value, "valid-token", TestContext.Current.CancellationToken);

        Assert.Same(player, user.Player);
        _clashOfClansService.Verify(service => service.GetPlayerAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
        _playerRepository.Verify(repository => repository.CreateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()), Times.Never);
        _userRepository.Verify(repository => repository.UpdateAsync(user, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task LinkPlayerAsync_WhenPlayerIsNew_RetrievesAndCreatesPlayer()
    {
        User user = TestDataBuilder.BuildUser();
        Player player = TestDataBuilder.BuildPlayer();
        _userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clashOfClansService
            .Setup(service => service.VerifyPlayerTokenAsync(player.Tag, "valid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _playerRepository
            .Setup(repository => repository.FindByTagAsync(player.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);
        _clashOfClansService
            .Setup(service => service.GetPlayerAsync(player.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        await _userService.LinkPlayerAsync(user.Id, player.Tag.Value, "valid-token", TestContext.Current.CancellationToken);

        Assert.Same(player, user.Player);
        _playerRepository.Verify(repository => repository.CreateAsync(player, TestContext.Current.CancellationToken), Times.Once);
        _domainEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task UnlinkPlayerAsync_UnlinksPersistsAndDispatchesEvents()
    {
        Player player = TestDataBuilder.BuildPlayer();
        User user = TestDataBuilder.BuildUser(player);
        _userRepository
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await _userService.UnlinkPlayerAsync(user.Id, TestContext.Current.CancellationToken);

        Assert.Null(user.Player);
        _userRepository.Verify(repository => repository.UpdateAsync(user, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }
}
