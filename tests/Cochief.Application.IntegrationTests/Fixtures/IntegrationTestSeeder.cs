namespace Cochief.Application.IntegrationTests.Fixtures;

using Cochief.Domain.Enums;
using Cochief.Infrastructure.Persistence;
using Cochief.Infrastructure.Persistence.Entities;
using Cochief.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

internal static class IntegrationTestSeeder
{
    internal static readonly Guid UserId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    internal static readonly Guid PlayerId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    internal static readonly Guid ClanId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    internal static readonly Guid SessionId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    internal const string UserEmail = "existing@example.com";
    internal const string UserPassword = "password";
    internal const string PlayerTag = "#PLAYER1";
    internal const string ClanTag = "#CLAN1";
    internal const string RefreshToken = "existing-refresh-token";

    internal static async Task ResetAsync(IServiceProvider services, Action<CochiefDbContext>? seed = null, CancellationToken ct = default)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        CochiefDbContext dbContext = scope.ServiceProvider.GetRequiredService<CochiefDbContext>();
        await dbContext.Database.EnsureDeletedAsync(ct);
        await dbContext.Database.EnsureCreatedAsync(ct);

        if (seed is not null)
        {
            seed(dbContext);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    internal static void WithExistingUser(CochiefDbContext dbContext)
    {
        dbContext.Add(CreateUser());
    }

    internal static void WithLinkedPlayer(CochiefDbContext dbContext)
    {
        dbContext.AddRange(CreatePlayer(), CreateUser(PlayerId));
    }

    internal static void WithAccessibleClan(CochiefDbContext dbContext)
    {
        ClanEntity clan = new ClanEntity
        {
            Id = ClanId,
            Name = "Existing Clan",
            Tag = ClanTag
        };

        clan.Members.Add(new MemberEntity
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
            PlayerId = PlayerId,
            ClanId = ClanId,
            Role = MemberRole.Leader
        });

        dbContext.AddRange(CreatePlayer(), clan, CreateUser(PlayerId));
    }

    internal static void WithActiveSession(CochiefDbContext dbContext)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        dbContext.AddRange(CreateUser(),
            new UserSessionEntity
            {
                Id = SessionId,
                UserId = UserId,
                RefreshTokenHash = HashRefreshToken(RefreshToken),
                CreatedAt = now.AddHours(-1),
                ExpiresAt = now.AddDays(1)
            });
    }

    internal static string HashRefreshToken(string refreshToken)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }

    private static UserEntity CreateUser(Guid? playerId = null)
    {
        return new UserEntity
        {
            Id = UserId,
            Name = "Existing User",
            Email = UserEmail,
            PasswordHash = new Argon2idPasswordHasher().Hash(UserPassword),
            PlayerId = playerId
        };
    }

    private static PlayerEntity CreatePlayer()
    {
        return new PlayerEntity
        {
            Id = PlayerId,
            Name = "Existing Player",
            Tag = PlayerTag,
            TownHallLevel = 16,
            ClanTag = ClanTag
        };
    }
}
