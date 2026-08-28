namespace Cochief.Domain.Exceptions;

public sealed class InvalidEmailException : ValidationException
{
    private const string DefaultMessage = "Email is invalid.";

    public InvalidEmailException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
