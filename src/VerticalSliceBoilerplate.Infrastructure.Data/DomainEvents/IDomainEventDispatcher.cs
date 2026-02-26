using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Infrastructure.Data.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

