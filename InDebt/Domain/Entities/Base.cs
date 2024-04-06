namespace Domain.Entities;

public abstract class Base
{
    public Guid Id { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Base entity
               && entity.GetHashCode() == GetHashCode()
               && entity.Id == Id
               && entity.GetType() == GetType();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id.GetHashCode(), GetType().GetHashCode());
    }
}