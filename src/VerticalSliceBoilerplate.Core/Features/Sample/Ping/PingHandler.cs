using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Sample.Ping;

public sealed class PingHandler : IPingHandler
{
    private readonly TimeProvider _timeProvider;

    public PingHandler(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<Result<PingResponse>> HandleAsync(PingRequest request, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var response = new PingResponse
        {
            Message = string.IsNullOrWhiteSpace(request.Message) ? "Pong" : request.Message,
            UtcTimestamp = now
        };

        return Task.FromResult(Result.Success(response));
    }
}

