namespace Cochief.Infrastructure.ClashOfClans;

using System.Net.Http.Headers;

internal sealed class ClashOfClansAuthenticationHandler(ClashOfClansOptions options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.ApiToken))
        {
            throw new InvalidOperationException($"Configuration value '{ClashOfClansOptions.SectionName}:ApiToken' is required.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiToken);

        return base.SendAsync(request, cancellationToken);
    }
}
