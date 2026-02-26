namespace VerticalSliceBoilerplate.Shared;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = TimeProvider.System.GetUtcNow().DateTime;
    }

    protected DomainEvent(Guid id, DateTime occurredOnUtc)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; init; }
    public DateTime OccurredOnUtc { get; init; }
}

