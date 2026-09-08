namespace Cochief.Infrastructure.ClashOfClans;

using System.Net.Http.Headers;

internal sealed class ClashOfClansAuthenticationHandler(ClashOfClansOptions options) : DelegatingHandler
{
    private readonly ClashOfClansOptions _options = options;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiToken))
        {
            throw new InvalidOperationException($"Configuration value '{ClashOfClansOptions.SectionName}:ApiToken' is required.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);

        return base.SendAsync(request, cancellationToken);
    }
}
