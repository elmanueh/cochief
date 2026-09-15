namespace Cochief.Application.Exceptions;

public sealed class ClanNotFoundException(string message) : Exception(message)
{
}
