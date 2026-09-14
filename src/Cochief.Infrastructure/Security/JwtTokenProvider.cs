namespace Cochief.Infrastructure.Security;

using Cochief.Domain.Ports;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public sealed class JwtTokenProvider(IOptions<JwtOptions> options, TimeProvider timeProvider) : IAuthTokenProvider
{
    public const string SessionIdClaim = "sid";

    private readonly JwtOptions _options = options.Value;
    private readonly TimeProvider _timeProvider = timeProvider;

    public AuthTokenPair Issue(Guid userId, Guid sessionId)
    {
        DateTimeOffset issuedAt = _timeProvider.GetUtcNow();
        DateTimeOffset accessTokenExpiresAt = issuedAt.AddMinutes(_options.AccessTokenMinutes);
        DateTimeOffset refreshTokenExpiresAt = issuedAt.AddDays(_options.RefreshTokenDays);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(SessionIdClaim, sessionId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, issuedAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];

        SymmetricSecurityKey signingKey = new(Encoding.UTF8.GetBytes(_options.SigningKey));
        SigningCredentials credentials = new(signingKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: accessTokenExpiresAt.UtcDateTime,
            signingCredentials: credentials);

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

        return new AuthTokenPair(accessToken, accessTokenExpiresAt, refreshToken, refreshTokenExpiresAt);
    }

    public string HashRefreshToken(string refreshToken)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(hash);
    }
}
