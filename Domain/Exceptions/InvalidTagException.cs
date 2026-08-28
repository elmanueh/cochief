namespace Cochief.Domain.Exceptions;

public sealed class InvalidTagException : ValidationException
{
    private const string DefaultMessage = "Tag is invalid.";

    public InvalidTagException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
