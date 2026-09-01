using Cochief.Domain.Exceptions;
using System.Net.Mail;

namespace Cochief.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidEmailException("Email cannot be empty.");

        string normalizedValue = value.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalizedValue, out MailAddress? emailAddress) ||
            !string.Equals(emailAddress.Address, normalizedValue, StringComparison.OrdinalIgnoreCase) ||
            !emailAddress.Host.Contains('.', StringComparison.Ordinal))
        {
            throw new InvalidEmailException("Email must follow the local@domain.extension format.");
        }

        return new Email(normalizedValue);
    }

    public static Email Restore(string value) => new Email(value);

    public override string ToString() => Value;
}
