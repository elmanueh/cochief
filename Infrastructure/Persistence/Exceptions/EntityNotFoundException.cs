namespace Cochief.Infrastructure.Persistence.Exceptions;

public sealed class EntityNotFoundException(string message) : RepositoryException(message)
{
}
