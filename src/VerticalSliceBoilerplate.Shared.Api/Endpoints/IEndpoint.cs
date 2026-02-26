using Microsoft.AspNetCore.Routing;

namespace VerticalSliceBoilerplate.Shared.Api.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

