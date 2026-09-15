namespace Cochief.Application.Exceptions;

public sealed class ClanAccessDeniedException(string message) : Exception(message)
{
}
