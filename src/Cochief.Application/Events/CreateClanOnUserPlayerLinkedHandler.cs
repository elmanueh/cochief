namespace Cochief.Application.Events;

using Cochief.Domain.Events;
using Cochief.Domain.Ports;
using MediatR;

public sealed class CreateClanOnUserPlayerLinkedHandler(IClanService clanService) : INotificationHandler<UserPlayerLinkedEvent>
{
    private readonly IClanService _clanService = clanService;

    public async Task Handle(UserPlayerLinkedEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notification.ClanTag)) return;

        await _clanService.CreateAsync(notification.ClanTag, cancellationToken);
    }
}
