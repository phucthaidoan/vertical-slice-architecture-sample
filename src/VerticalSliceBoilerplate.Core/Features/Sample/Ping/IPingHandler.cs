using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Sample.Ping;

public interface IPingHandler
{
    Task<Result<PingResponse>> HandleAsync(PingRequest request, CancellationToken cancellationToken = default);
}

