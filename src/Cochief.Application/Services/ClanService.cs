namespace Cochief.Application.Services;

using Cochief.Domain.Enums;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;
using Cochief.Domain.Shared;
using Cochief.Domain.ValueObjects;

public sealed class ClanService(IClanRepository clanRepository, IPlayerRepository playerRepository, IClashOfClansService clashOfClansService, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEvents) : IClanService
{
    private readonly IClanRepository _clanRepository = clanRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IClashOfClansService _clashOfClansService = clashOfClansService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IDomainEventDispatcher _domainEvents = domainEvents;

    public async Task CreateAsync(string clanTag, CancellationToken ct)
    {
        Tag tag = Tag.Create(clanTag);

        Clan? existingClan = await _clanRepository.FindByTagAsync(tag, ct);
        if (existingClan is not null) return;

        Clan clan = await _clashOfClansService.GetClanAsync(tag, ct);

        await _clanRepository.CreateAsync(clan, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(clan.PullDomainEvents(), ct);
    }

    public async Task UpdateAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Clan> clans = await _clanRepository.GetAllAsync(ct);

        foreach (Clan clan in clans)
        {
            await UpdateAsync(clan.Id, ct);
        }
    }

    // TODO: refactor this method with domain events
    public async Task UpdateAsync(Guid clanId, CancellationToken ct)
    {
        Clan clan = await _clanRepository.GetByIdAsync(clanId, ct);
        IReadOnlyDictionary<Player, MemberRole> incomingMembers = await _clashOfClansService.GetClanMembersAsync(clan.Tag, ct);
        HashSet<Guid> currentPlayerIds = [];
        HashSet<AggregateRoot> affectedAggregates = [clan];

        foreach ((Player incomingPlayer, MemberRole role) in incomingMembers)
        {
            Player? player = await _playerRepository.FindByTagAsync(incomingPlayer.Tag, ct);

            if (player is null)
            {
                player = incomingPlayer;
                player.UpdateClanTag(clan.Tag.Value);
                await _playerRepository.CreateAsync(player, ct);
                affectedAggregates.Add(player);
            }
            else
            {
                Tag? previousClanTag = player.ClanTag;
                bool playerChanged = false;

                if (!string.Equals(player.Name, incomingPlayer.Name, StringComparison.Ordinal))
                {
                    player.UpdateName(incomingPlayer.Name);
                    playerChanged = true;
                }

                if (player.TownHallLevel != incomingPlayer.TownHallLevel)
                {
                    player.UpdateTownHallLevel(incomingPlayer.TownHallLevel);
                    playerChanged = true;
                }

                if (player.ClanTag != clan.Tag)
                {
                    if (previousClanTag is not null)
                    {
                        Clan? previousClan = await _clanRepository.FindByTagAsync(previousClanTag, ct);
                        if (previousClan?.Members.Any(member => member.PlayerId == player.Id) == true)
                        {
                            previousClan.DeleteMember(player.Id);
                            await _clanRepository.UpdateAsync(previousClan, ct);
                            affectedAggregates.Add(previousClan);
                        }
                    }

                    player.UpdateClanTag(clan.Tag.Value);
                    playerChanged = true;
                }

                if (playerChanged)
                {
                    await _playerRepository.UpdateAsync(player, ct);
                    affectedAggregates.Add(player);
                }
            }

            currentPlayerIds.Add(player.Id);
            Member? member = clan.Members.FirstOrDefault(member => member.PlayerId == player.Id);
            if (member is null)
            {
                clan.AddMember(player.Id, role);
            }
            else if (member.Role != role)
            {
                clan.UpdateMember(player.Id, role);
            }
        }

        foreach (Guid formerPlayerId in clan.Members.Select(member => member.PlayerId).Where(playerId => !currentPlayerIds.Contains(playerId)).ToArray())
        {
            clan.DeleteMember(formerPlayerId);

            Player formerPlayer = await _playerRepository.GetByIdAsync(formerPlayerId, ct);
            if (formerPlayer.ClanTag == clan.Tag)
            {
                formerPlayer.UpdateClanTag(null);
                await _playerRepository.UpdateAsync(formerPlayer, ct);
                affectedAggregates.Add(formerPlayer);
            }
        }

        await _clanRepository.UpdateAsync(clan, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _domainEvents.DispatchAsync(affectedAggregates.SelectMany(aggregate => aggregate.PullDomainEvents()), ct);
    }
}
