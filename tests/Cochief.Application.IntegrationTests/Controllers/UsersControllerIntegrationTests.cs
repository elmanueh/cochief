namespace Cochief.Application.IntegrationTests.Controllers;

using Cochief.Api.Presentation.Dtos;
using Cochief.Application.IntegrationTests.Fixtures;
using Cochief.Domain.Model;
using System.Net;
using System.Net.Http.Json;

public sealed class UsersControllerIntegrationTests(PostgreSqlTestContainerFactory factory) : IClassFixture<PostgreSqlTestContainerFactory>
{
    [Fact]
    public async Task GetMe_WithPersistedUser_ReturnsCurrentUser()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithExistingUser, ct);
        using HttpClient client = factory.CreateAuthenticatedClient(IntegrationTestSeeder.UserId);

        using HttpResponseMessage response = await client.GetAsync("/api/users/me", ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        UserResponseDto? user = await response.Content.ReadFromJsonAsync<UserResponseDto>(ct);
        Assert.Equal(IntegrationTestSeeder.UserId, user?.Id);
        Assert.Equal(IntegrationTestSeeder.UserEmail, user?.Email);
    }

    [Fact]
    public async Task LinkPlayer_WithValidVerification_LinksPlayer()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithExistingUser, ct);
        factory.ClashOfClans.VerificationResult = true;
        factory.ClashOfClans.Player = Player.Restore(IntegrationTestSeeder.PlayerId, "Linked Player", IntegrationTestSeeder.PlayerTag, 16, IntegrationTestSeeder.ClanTag);
        factory.ClashOfClans.Clan = Clan.Restore(IntegrationTestSeeder.ClanId, "Linked Clan", IntegrationTestSeeder.ClanTag);
        using HttpClient client = factory.CreateAuthenticatedClient(IntegrationTestSeeder.UserId);
        CreateLinkPlayerRequestDto request = new CreateLinkPlayerRequestDto()
        {
            PlayerTag = IntegrationTestSeeder.PlayerTag,
            VerificationToken = "valid-token"
        };

        using HttpResponseMessage response = await client.PatchAsJsonAsync("/api/users/me/link", request, ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        UserResponseDto user = await GetCurrentUserAsync(client, ct);
        UserResponseDto.PlayerResponseDto player = Assert.Single(user.Players);
        Assert.Equal(IntegrationTestSeeder.PlayerId, player.Id);
        Assert.Equal(IntegrationTestSeeder.ClanTag, player.ClanTag);
    }

    [Fact]
    public async Task UnlinkPlayer_WithPersistedPlayer_UnlinksPlayer()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithLinkedPlayer, ct);
        using HttpClient client = factory.CreateAuthenticatedClient(IntegrationTestSeeder.UserId);

        using HttpResponseMessage response = await client.PatchAsync("/api/users/me/unlink", content: null, ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        UserResponseDto user = await GetCurrentUserAsync(client, ct);
        Assert.Empty(user.Players);
    }

    private static async Task<UserResponseDto> GetCurrentUserAsync(HttpClient client, CancellationToken ct)
    {
        using HttpResponseMessage response = await client.GetAsync("/api/users/me", ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        UserResponseDto? user = await response.Content.ReadFromJsonAsync<UserResponseDto>(ct);
        return Assert.IsType<UserResponseDto>(user);
    }
}
