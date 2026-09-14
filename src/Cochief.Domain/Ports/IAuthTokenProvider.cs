namespace Cochief.Domain.Ports;

public interface IAuthTokenProvider
{
    AuthTokenPair Issue(Guid userId, Guid sessionId);

    string HashRefreshToken(string refreshToken);
}
