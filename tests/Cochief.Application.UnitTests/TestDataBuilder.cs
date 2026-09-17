namespace Cochief.Application.UnitTests;

using Cochief.Domain.Model;
using Cochief.Domain.Ports;

internal static class TestDataBuilder
{
    internal static User BuildUser(Player? player = null)
    {
        User user = User.Restore(
            id: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            name: "Test User",
            email: "test@example.com",
            passwordHash: "stored-password-hash",
            player: player);

        return user;
    }

    internal static Player BuildPlayer(string tag = "#PLAYER1", string? clanTag = "#CLAN1", string name = "Player One", int townHallLevel = 16)
    {
        Player player = Player.Restore(
            id: Guid.Parse("20000000-0000-0000-0000-000000000001"),
            name: name,
            tag: tag,
            townHallLevel: townHallLevel,
            clanTag: clanTag);

        return player;
    }

    internal static Clan BuildClan(string tag = "#CLAN1", IEnumerable<Member>? members = null, Guid? id = null)
    {
        Clan clan = Clan.Restore(
            id: id ?? Guid.Parse("30000000-0000-0000-0000-000000000001"),
            name: "Test Clan",
            tag: tag,
            members: members);

        return clan;
    }

    internal static AuthTokenPair BuildTokenPair(DateTimeOffset now)
    {
        AuthTokenPair tokenPair = new AuthTokenPair(
            accessToken: "access-token",
            accessTokenExpiresAt: now.AddMinutes(15),
            refreshToken: "refresh-token",
            refreshTokenExpiresAt: now.AddDays(7));

        return tokenPair;
    }
}

internal sealed class StubTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
