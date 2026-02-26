using Microsoft.EntityFrameworkCore.Diagnostics;
using VerticalSliceBoilerplate.Infrastructure.Data.DomainEvents;
using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Infrastructure.Data.Interceptors;

public class DomainEventInterceptor(IDomainEventDispatcher domainEventDispatcher) : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is not null)
        {
            var domainEvents = context.ChangeTracker
                .Entries()
                .Where(e => e.Entity is IHasDomainEvents)
                .SelectMany(e => ((IHasDomainEvents)e.Entity).DomainEvents)
                .ToList();

            foreach (var entry in context.ChangeTracker.Entries().Where(e => e.Entity is IHasDomainEvents))
            {
                ((IHasDomainEvents)entry.Entity).ClearDomainEvents();
            }

            if (domainEvents.Any())
            {
                await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}

