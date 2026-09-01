namespace Cochief.Infrastructure.ClashOfClans.Exceptions;

public sealed class ClashOfClansException : Exception
{
    public int? ApiStatusCode { get; }

    public ClashOfClansException(string message, int? apiStatusCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ApiStatusCode = apiStatusCode;
    }
}
