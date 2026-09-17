namespace Cochief.Application.IntegrationTests.Controllers;

using Cochief.Api.Presentation.Dtos;
using Cochief.Application.IntegrationTests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public sealed class AuthControllerIntegrationTests(PostgreSqlTestContainerFactory factory) : IClassFixture<PostgreSqlTestContainerFactory>
{
    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreatedUser()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(ct: ct);
        using HttpClient client = factory.CreateClient();
        CreateUserRequestDto request = new CreateUserRequestDto()
        {
            Name = "New User",
            Email = "new@example.com",
            Password = "password"
        };

        using HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", request, ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        UserResponseDto? user = await response.Content.ReadFromJsonAsync<UserResponseDto>(ct);
        Assert.NotNull(user);
        Assert.Equal("New User", user.Name);
        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public async Task Login_WithPersistedUser_ReturnsAuthenticationTokens()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithExistingUser, ct);
        using HttpClient client = factory.CreateClient();
        CreateLoginRequestDto request = new CreateLoginRequestDto()
        {
            Email = IntegrationTestSeeder.UserEmail,
            Password = IntegrationTestSeeder.UserPassword
        };

        using HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", request, ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthenticationResponseDto? authentication = await response.Content.ReadFromJsonAsync<AuthenticationResponseDto>(ct);
        Assert.NotNull(authentication);
        Assert.NotEmpty(authentication.AccessToken);
        Assert.NotEmpty(authentication.RefreshToken);
    }

    [Fact]
    public async Task Refresh_WithPersistedSession_ReturnsRotatedTokens()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithActiveSession, ct);
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestSeeder.RefreshToken);

        using HttpResponseMessage response = await client.PostAsync("/api/auth/refresh", content: null, ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthenticationResponseDto? authentication = await response.Content.ReadFromJsonAsync<AuthenticationResponseDto>(ct);
        Assert.NotNull(authentication);
        Assert.NotEmpty(authentication.AccessToken);
        Assert.NotEqual(IntegrationTestSeeder.RefreshToken, authentication.RefreshToken);
    }

    [Fact]
    public async Task Logout_WithPersistedSession_RevokesSession()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        await factory.ResetDatabaseAsync(IntegrationTestSeeder.WithActiveSession, ct);
        using HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", IntegrationTestSeeder.RefreshToken);

        using HttpResponseMessage response = await client.PostAsync("/api/auth/logout", content: null, ct);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using HttpResponseMessage refreshResponse = await client.PostAsync("/api/auth/refresh", content: null, ct);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }
}
