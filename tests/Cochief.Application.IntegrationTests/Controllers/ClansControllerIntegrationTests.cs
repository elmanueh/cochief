namespace Cochief.Application.IntegrationTests.Controllers;

using Cochief.Api.Presentation.Dtos;
using Cochief.Application.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Json;

public sealed class ClansControllerIntegrationTests(PostgreSqlTestContainerFactory factory) : IClassFixture<PostgreSqlTestContainerFactory>
{
    [Fact]
    public async Task Get_WithAccessibleClan_ReturnsClanCollection()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithAccessibleClan, ct);
        using HttpClient client = factory.CreateAuthenticatedClient(IntegrationTestSeeder.UserId);

        using HttpResponseMessage response = await client.GetAsync("/api/clans", ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        IReadOnlyList<ClanResponseDto>? clans = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClanResponseDto>>(ct);
        ClanResponseDto clan = Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<ClanResponseDto>>(clans));
        Assert.Equal(IntegrationTestSeeder.ClanId, clan.Id);
        Assert.Equal(IntegrationTestSeeder.ClanTag, clan.Tag);
    }

    [Fact]
    public async Task GetByTag_WithAccessibleClan_ReturnsClanAndMembers()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithAccessibleClan, ct);
        using HttpClient client = factory.CreateAuthenticatedClient(IntegrationTestSeeder.UserId);
        string encodedTag = Uri.EscapeDataString(IntegrationTestSeeder.ClanTag);

        using HttpResponseMessage response = await client.GetAsync($"/api/clans/{encodedTag}", ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        ClanResponseDto? clan = await response.Content.ReadFromJsonAsync<ClanResponseDto>(ct);
        Assert.NotNull(clan);
        Assert.Equal(IntegrationTestSeeder.ClanId, clan.Id);
        Assert.Equal(IntegrationTestSeeder.ClanTag, clan.Tag);
        ClanResponseDto.ClanMemberResponseDto member = Assert.Single(clan.Members);
        Assert.Equal(IntegrationTestSeeder.PlayerId, member.PlayerId);
        Assert.Equal("Leader", member.Role);
    }
}
