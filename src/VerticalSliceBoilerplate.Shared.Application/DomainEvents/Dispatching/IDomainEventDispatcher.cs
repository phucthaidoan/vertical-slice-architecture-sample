using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Shared.Application.DomainEvents.Dispatching;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

