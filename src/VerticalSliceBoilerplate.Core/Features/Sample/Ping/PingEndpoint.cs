using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VerticalSliceBoilerplate.Shared.Api;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Sample.Ping;

public sealed class PingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/sample/ping", async (string? message, IPingHandler handler, CancellationToken ct) =>
            {
                var request = new PingRequest { Message = message ?? "Ping" };
                var result = await handler.HandleAsync(request, ct);
                return ApiResults.ToApiResponse(result);
            })
            .WithTags(Sample.SampleTags.Sample)
            .WithSummary("Sample ping endpoint")
            .WithDescription("Simple sample endpoint that echoes a message and returns the current UTC timestamp.")
            .Produces<ApiResponse<PingResponse>>(StatusCodes.Status200OK);
    }
}

