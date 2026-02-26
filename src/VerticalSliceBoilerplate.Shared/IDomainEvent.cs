namespace VerticalSliceBoilerplate.Shared;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOnUtc { get; }
}

