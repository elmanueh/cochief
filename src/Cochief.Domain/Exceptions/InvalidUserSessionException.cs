namespace Cochief.Domain.Exceptions;

public sealed class InvalidUserSessionException(string message) : DomainException(message)
{
}
