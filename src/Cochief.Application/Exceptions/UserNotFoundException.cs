namespace Cochief.Application.Exceptions;

public sealed class UserNotFoundException(string message) : Exception(message)
{
}
