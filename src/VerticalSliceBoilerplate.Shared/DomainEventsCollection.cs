namespace VerticalSliceBoilerplate.Shared;

public sealed class DomainEventsCollection : IHasDomainEvents
{
    private readonly List<IDomainEvent> _events = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();

    public void Raise(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _events.Clear();
    }
}

