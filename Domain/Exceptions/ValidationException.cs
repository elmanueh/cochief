namespace Cochief.Domain.Exceptions;

public abstract class ValidationException : DomainException
{
    protected ValidationException(string message)
        : base(message)
    {
    }
}
