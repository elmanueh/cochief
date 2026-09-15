namespace Cochief.Api.Presentation.Dtos;

/// <summary>Tokens issued for an authenticated session.</summary>
public sealed class AuthenticationResponseDto
{
    /// <summary>JWT used to authorize API requests.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>UTC instant when the access token expires.</summary>
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    /// <summary>Token used to renew the authentication session.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>UTC instant when the refresh token expires.</summary>
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}
