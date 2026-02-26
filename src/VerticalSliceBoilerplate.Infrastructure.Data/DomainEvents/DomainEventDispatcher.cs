using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Infrastructure.Data.DomainEvents;

public class DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchSingleEventAsync(domainEvent, cancellationToken);
        }
    }

    private async Task DispatchSingleEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is IDomainEventHandler domainEventHandler)
            {
                try
                {
                    await domainEventHandler.Handle(domainEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling domain event {EventType} with handler {HandlerType}",
                        eventType.Name, handler.GetType().Name);
                    throw;
                }
            }
        }
    }
}

