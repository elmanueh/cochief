namespace Cochief.Application.IntegrationTests.Fixtures;

using Cochief.Domain.Ports;
using Cochief.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using Testcontainers.PostgreSql;

public sealed class PostgreSqlTestContainerFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSql = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("cochief_integration_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    internal TestClashOfClansService ClashOfClans { get; } = new TestClashOfClansService();

    public async ValueTask InitializeAsync()
    {
        await _postgreSql.StartAsync();
        _ = CreateClient();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgreSql.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSql"] = _postgreSql.GetConnectionString(),
                ["Authentication:Jwt:Issuer"] = "cochief-integration-tests",
                ["Authentication:Jwt:Audience"] = "cochief-integration-tests",
                ["Authentication:Jwt:SigningKey"] = "integration-tests-signing-key-at-least-32-bytes",
                ["Authentication:Jwt:AccessTokenMinutes"] = "15",
                ["Authentication:Jwt:RefreshTokenDays"] = "7",
                ["ClashOfClans:BaseUrl"] = "https://example.test/"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<CochiefDbContext>>();
            services.RemoveAll<CochiefDbContext>();
            services.AddDbContext<CochiefDbContext>(options => options.UseNpgsql(_postgreSql.GetConnectionString(), npgsql => npgsql.EnableRetryOnFailure()));

            services.RemoveAll<IClashOfClansService>();
            services.AddSingleton<IClashOfClansService>(ClashOfClans);

            ServiceDescriptor? synchronizationWorker = services.FirstOrDefault(descriptor =>
                descriptor.ServiceType == typeof(IHostedService) &&
                descriptor.ImplementationType?.Name == "ClanSynchronizationWorker");

            if (synchronizationWorker is not null) services.Remove(synchronizationWorker);
        });
    }

    internal async Task ResetDatabaseAsync(Action<CochiefDbContext>? seed = null, CancellationToken ct = default)
    {
        ClashOfClans.Reset();
        await IntegrationTestSeeder.ResetAsync(Services, seed, ct);
    }

    internal HttpClient CreateAuthenticatedClient(Guid userId)
    {
        HttpClient client = CreateClient();
        using IServiceScope scope = Services.CreateScope();
        IAuthTokenProvider tokens = scope.ServiceProvider.GetRequiredService<IAuthTokenProvider>();
        string accessToken = tokens.Issue(userId, Guid.NewGuid()).AccessToken;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return client;
    }
}
