using Cochief.Domain.Services;
using Isopoh.Cryptography.Argon2;

namespace Cochief.Infrastructure.Security;

public sealed class Argon2idPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return Argon2.Hash(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash)) return false;

        return Argon2.Verify(passwordHash, password);
    }
}
