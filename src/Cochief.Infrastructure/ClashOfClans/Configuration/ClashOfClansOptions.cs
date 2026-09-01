namespace Cochief.Infrastructure.ClashOfClans;

internal sealed class ClashOfClansOptions
{
    public const string SectionName = "ClashOfClans";
    public const string DefaultBaseUrl = "https://api.clashofclans.com/v1/";

    public string BaseUrl { get; init; } = DefaultBaseUrl;
    public string ApiToken { get; init; } = string.Empty;
}
