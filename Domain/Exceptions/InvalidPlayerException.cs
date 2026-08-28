namespace Cochief.Domain.Exceptions;

public sealed class InvalidPlayerException : ValidationException
{
    private const string DefaultMessage = "Player is invalid.";

    public InvalidPlayerException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
