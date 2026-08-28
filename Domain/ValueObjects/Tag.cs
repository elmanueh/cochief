using Cochief.Domain.Exceptions;

namespace Cochief.Domain.ValueObjects;

public sealed record Tag
{
    private string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static Tag Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidTagException("Tag cannot be empty.");
        if (!value.StartsWith('#')) throw new InvalidTagException("Tag must start with a '#' character.");

        string normalizedValue = value.Trim().ToUpperInvariant();

        return new Tag(normalizedValue);
    }

    public override string ToString() => Value;
}
