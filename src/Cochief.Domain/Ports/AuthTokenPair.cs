namespace Cochief.Domain.Ports;

public sealed class AuthTokenPair
{
    public string AccessToken { get; }
    public DateTimeOffset AccessTokenExpiresAt { get; }
    public string RefreshToken { get; }
    public DateTimeOffset RefreshTokenExpiresAt { get; }

    public AuthTokenPair(string accessToken, DateTimeOffset accessTokenExpiresAt, string refreshToken, DateTimeOffset refreshTokenExpiresAt)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }
}
