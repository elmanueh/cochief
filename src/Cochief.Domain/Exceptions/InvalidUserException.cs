namespace Cochief.Domain.Exceptions;

public sealed class InvalidUserException : ValidationException
{
    private const string DefaultMessage = "User is invalid.";

    public InvalidUserException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
