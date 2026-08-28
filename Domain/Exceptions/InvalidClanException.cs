namespace Cochief.Domain.Exceptions;

public sealed class InvalidClanException : ValidationException
{
    private const string DefaultMessage = "Clan is invalid.";

    public InvalidClanException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
