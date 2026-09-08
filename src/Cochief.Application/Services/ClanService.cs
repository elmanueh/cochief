namespace Cochief.Application.Services;

using Cochief.Domain.Enums;
using Cochief.Domain.Model;
using Cochief.Domain.Ports;

public sealed class ClanService(IClanRepository clanRepository, IPlayerRepository playerRepository, IClashOfClansService clashOfClansService, IUnitOfWork unitOfWork) : IClanService
{
    private readonly IClanRepository _clanRepository = clanRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IClashOfClansService _clashOfClansService = clashOfClansService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        foreach ((Player incomingPlayer, MemberRole role) in incomingMembers)
        {
            Player? player = await _playerRepository.FindByTagAsync(incomingPlayer.Tag, ct);

            if (player is null)
            {
                player = incomingPlayer;
                player.UpdateClanId(clan.Id);
                await _playerRepository.CreateAsync(player, ct);
            }
            else
            {
                Guid? previousClanId = player.ClanId;
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

                if (player.ClanId != clan.Id)
                {
                    if (previousClanId.HasValue)
                    {
                        Clan previousClan = await _clanRepository.GetByIdAsync(previousClanId.Value, ct);
                        previousClan.DeleteMember(player.Id);
                        await _clanRepository.UpdateAsync(previousClan, ct);
                    }

                    player.UpdateClanId(clan.Id);
                    playerChanged = true;
                }

                if (playerChanged)
                {
                    await _playerRepository.UpdateAsync(player, ct);
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
            if (formerPlayer.ClanId == clan.Id)
            {
                formerPlayer.UpdateClanId(null);
                await _playerRepository.UpdateAsync(formerPlayer, ct);
            }
        }

        await _clanRepository.UpdateAsync(clan, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
