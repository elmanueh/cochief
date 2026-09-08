namespace Cochief.Domain.Shared;

public abstract class Entity
{
    public Guid Id { get; }

    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
