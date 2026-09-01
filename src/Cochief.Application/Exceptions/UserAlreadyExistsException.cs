namespace Cochief.Application.Exceptions;

public sealed class UserAlreadyExistsException(string message) : Exception(message)
{
}
