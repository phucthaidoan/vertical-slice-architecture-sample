namespace VerticalSliceBoilerplate.Core.Features.Sample.Ping;

public sealed class PingResponse
{
    public string Message { get; init; } = string.Empty;
    public DateTime UtcTimestamp { get; init; }
}

