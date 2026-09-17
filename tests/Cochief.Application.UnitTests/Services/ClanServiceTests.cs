namespace Cochief.Application.UnitTests.Services;

using Cochief.Application.Exceptions;
using Cochief.Application.Services;
using Cochief.Application.UnitTests.Fixtures;
using Cochief.Domain.Enums;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;
using Moq;

public sealed class ClanServiceTests : AutoMoqTest
{
    private readonly Mock<IClanRepository> _clans;
    private readonly Mock<IPlayerRepository> _players;
    private readonly Mock<IUserRepository> _users;
    private readonly Mock<IClashOfClansService> _clashOfClans;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IDomainEventDispatcher> _domainEvents;
    private readonly ClanService _clanService;

    public ClanServiceTests()
    {
        _clans = Freeze<IClanRepository>();
        _players = Freeze<IPlayerRepository>();
        _users = Freeze<IUserRepository>();
        _clashOfClans = Freeze<IClashOfClansService>();
        _unitOfWork = Freeze<IUnitOfWork>();
        _domainEvents = Freeze<IDomainEventDispatcher>();
        _clanService = Create<ClanService>();
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenUserHasNoPlayer_ReturnsEmptyList()
    {
        User user = TestDataBuilder.BuildUser();
        _users
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        IReadOnlyList<Clan> result = await _clanService.GetByUserIdAsync(user.Id, TestContext.Current.CancellationToken);

        Assert.Empty(result);
        _clans.Verify(repository => repository.FindByTagAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenClanExists_ReturnsClan()
    {
        Player player = TestDataBuilder.BuildPlayer();
        User user = TestDataBuilder.BuildUser(player);
        Clan clan = TestDataBuilder.BuildClan(player.ClanTag!.Value);
        _users
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clans
            .Setup(repository => repository.FindByTagAsync(player.ClanTag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);

        IReadOnlyList<Clan> result = await _clanService.GetByUserIdAsync(user.Id, TestContext.Current.CancellationToken);

        Assert.Collection(result, returnedClan => Assert.Same(clan, returnedClan));
    }

    [Fact]
    public async Task GetByTagAsync_WhenUserBelongsToAnotherClan_ThrowsBeforeLoadingClan()
    {
        User user = TestDataBuilder.BuildUser(TestDataBuilder.BuildPlayer(clanTag: "#CLAN1"));
        _users
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        Task Act() => _clanService.GetByTagAsync(user.Id, "#CLAN2", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<ClanAccessDeniedException>(Act);
        _clans.Verify(repository => repository.FindByTagAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByTagAsync_WhenClanDoesNotExist_Throws()
    {
        User user = TestDataBuilder.BuildUser(TestDataBuilder.BuildPlayer(clanTag: "#CLAN1"));
        _users
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clans
            .Setup(repository => repository.FindByTagAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Clan?)null);

        Task Act() => _clanService.GetByTagAsync(user.Id, "#CLAN1", TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<ClanNotFoundException>(Act);
    }

    [Fact]
    public async Task GetByTagAsync_WhenUserHasAccess_ReturnsClan()
    {
        User user = TestDataBuilder.BuildUser(TestDataBuilder.BuildPlayer(clanTag: "#CLAN1"));
        Clan clan = TestDataBuilder.BuildClan();
        _users
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _clans
            .Setup(repository => repository.FindByTagAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);

        Clan result = await _clanService.GetByTagAsync(user.Id, clan.Tag.Value, TestContext.Current.CancellationToken);

        Assert.Same(clan, result);
    }

    [Fact]
    public async Task CreateAsync_WhenClanAlreadyExists_DoesNothing()
    {
        Clan clan = TestDataBuilder.BuildClan();
        _clans
            .Setup(repository => repository.FindByTagAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);

        await _clanService.CreateAsync(clan.Tag.Value, TestContext.Current.CancellationToken);

        _clashOfClans.Verify(service => service.GetClanAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenClanIsNew_RetrievesPersistsAndDispatchesEvents()
    {
        Clan clan = Clan.Create("New Clan", "#NEWCLAN");
        _clans
            .Setup(repository => repository.FindByTagAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Clan?)null);
        _clashOfClans
            .Setup(service => service.GetClanAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);

        await _clanService.CreateAsync(clan.Tag.Value, TestContext.Current.CancellationToken);

        _clans.Verify(repository => repository.CreateAsync(clan, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
        _domainEvents.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 1), TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAllAsync_UpdatesEveryClan()
    {
        Clan firstClan = TestDataBuilder.BuildClan("#CLAN1");
        Clan secondClan = TestDataBuilder.BuildClan(tag: "#CLAN2", id: Guid.Parse("30000000-0000-0000-0000-000000000002"));
        _clans
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([firstClan, secondClan]);
        _clans
            .Setup(repository => repository.GetByIdAsync(firstClan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(firstClan);
        _clans
            .Setup(repository => repository.GetByIdAsync(secondClan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(secondClan);
        _clashOfClans
            .Setup(service => service.GetClanMembersAsync(It.IsAny<Tag>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Player, MemberRole>());

        await _clanService.UpdateAllAsync(TestContext.Current.CancellationToken);

        _clans.Verify(repository => repository.UpdateAsync(firstClan, TestContext.Current.CancellationToken), Times.Once);
        _clans.Verify(repository => repository.UpdateAsync(secondClan, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateAsync_WhenIncomingPlayerIsNew_CreatesPlayerAndClanMember()
    {
        Clan clan = TestDataBuilder.BuildClan();
        Player incomingPlayer = TestDataBuilder.BuildPlayer(clanTag: null);
        _clans
            .Setup(repository => repository.GetByIdAsync(clan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);
        _clashOfClans
            .Setup(service => service.GetClanMembersAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Player, MemberRole> { [incomingPlayer] = MemberRole.Elder });
        _players
            .Setup(repository => repository.FindByTagAsync(incomingPlayer.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        await _clanService.UpdateAsync(clan.Id, TestContext.Current.CancellationToken);

        Assert.Equal(clan.Tag, incomingPlayer.ClanTag);
        Assert.Contains(clan.Members, member => member.PlayerId == incomingPlayer.Id && member.Role == MemberRole.Elder);
        _players.Verify(repository => repository.CreateAsync(incomingPlayer, TestContext.Current.CancellationToken), Times.Once);
        _clans.Verify(repository => repository.UpdateAsync(clan, TestContext.Current.CancellationToken), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenExistingPlayerChangesClan_UpdatesBothClansAndPlayer()
    {
        Player player = TestDataBuilder.BuildPlayer(clanTag: "#OLDCLAN", name: "Old Name", townHallLevel: 15);
        Clan previousClan = TestDataBuilder.BuildClan("#OLDCLAN");
        previousClan.AddMember(player.Id, MemberRole.Member);
        previousClan.PullDomainEvents();
        Clan currentClan = TestDataBuilder.BuildClan(tag: "#CLAN1", id: Guid.Parse("30000000-0000-0000-0000-000000000002"));
        Player incomingPlayer = TestDataBuilder.BuildPlayer(player.Tag.Value, currentClan.Tag.Value, "New Name", 16);
        _clans
            .Setup(repository => repository.GetByIdAsync(currentClan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentClan);
        _clashOfClans
            .Setup(service => service.GetClanMembersAsync(currentClan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Player, MemberRole> { [incomingPlayer] = MemberRole.CoLeader });
        _players
            .Setup(repository => repository.FindByTagAsync(player.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);
        _clans
            .Setup(repository => repository.FindByTagAsync(previousClan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(previousClan);

        await _clanService.UpdateAsync(currentClan.Id, TestContext.Current.CancellationToken);

        Assert.Equal("New Name", player.Name);
        Assert.Equal(16, player.TownHallLevel);
        Assert.Equal(currentClan.Tag, player.ClanTag);
        Assert.Empty(previousClan.Members);
        Assert.Contains(currentClan.Members, member => member.PlayerId == player.Id && member.Role == MemberRole.CoLeader);
        _players.Verify(repository => repository.UpdateAsync(player, TestContext.Current.CancellationToken), Times.Once);
        _clans.Verify(repository => repository.UpdateAsync(previousClan, TestContext.Current.CancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMemberHasLeft_RemovesMemberAndClearsPlayerClan()
    {
        Player formerPlayer = TestDataBuilder.BuildPlayer();
        Clan clan = TestDataBuilder.BuildClan();
        clan.AddMember(formerPlayer.Id, MemberRole.Member);
        clan.PullDomainEvents();
        _clans
            .Setup(repository => repository.GetByIdAsync(clan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clan);
        _clashOfClans
            .Setup(service => service.GetClanMembersAsync(clan.Tag, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Player, MemberRole>());
        _players
            .Setup(repository => repository.GetByIdAsync(formerPlayer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(formerPlayer);

        await _clanService.UpdateAsync(clan.Id, TestContext.Current.CancellationToken);

        Assert.Empty(clan.Members);
        Assert.Null(formerPlayer.ClanTag);
        _players.Verify(repository => repository.UpdateAsync(formerPlayer, TestContext.Current.CancellationToken), Times.Once);
        _clans.Verify(repository => repository.UpdateAsync(clan, TestContext.Current.CancellationToken), Times.Once);
        _domainEvents.Verify(dispatcher => dispatcher.DispatchAsync(It.Is<IEnumerable<IDomainEvent>>(events => events.Count() == 2), TestContext.Current.CancellationToken), Times.Once);
    }
}
