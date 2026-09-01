namespace Cochief.Domain.Exceptions;

public sealed class InvalidMemberException : ValidationException
{
    private const string DefaultMessage = "Member is invalid.";

    public InvalidMemberException(string? message = null)
        : base(ResolveMessage(message))
    {
    }

    private static string ResolveMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? DefaultMessage : message;
}
