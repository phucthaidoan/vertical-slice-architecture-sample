namespace VerticalSliceBoilerplate.Shared;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void Raise(IDomainEvent domainEvent);
    void ClearDomainEvents();
}

