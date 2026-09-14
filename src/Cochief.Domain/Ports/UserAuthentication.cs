namespace Cochief.Domain.Ports;

using Cochief.Domain.Model;

public sealed class UserAuthentication
{
    public User User { get; }
    public Guid SessionId { get; }
    public AuthTokenPair Tokens { get; }

    public UserAuthentication(User user, Guid sessionId, AuthTokenPair tokens)
    {
        User = user;
        SessionId = sessionId;
        Tokens = tokens;
    }
}
