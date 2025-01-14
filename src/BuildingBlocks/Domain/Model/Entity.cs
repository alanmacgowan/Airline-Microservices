namespace BuildingBlocks.Domain.Model;

public abstract class Entity<TId> : IEntity<TId>
{
    protected Entity(TId id) => Id = id;
    protected Entity() { }

    public TId Id { get; protected set; }
    public DateTime LastModified { get; protected set;}
    public bool IsDeleted { get; protected set;}
    public int? ModifiedBy { get; protected set;}
}
